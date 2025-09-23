using System.Net;
using System.Net.Http.Json;
using ApiGateway.Models;

namespace ApiGateway.Services;

public interface IOrderAggregateService
{
    Task<PagedOrdersResponse> GetAllOrdersAsync(int offset, int limit);
    Task<OrderDetailsResponse?> GetOrderByIdAsync(long id);
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderDetailsResponse?> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request);
}

public class OrderAggregateService : IOrderAggregateService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<OrderAggregateService> _logger;

    public OrderAggregateService(IHttpClientFactory httpClientFactory, ILogger<OrderAggregateService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<PagedOrdersResponse> GetAllOrdersAsync(int offset, int limit)
    {
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        var pagedOrders = await ordersClient.GetFromJsonAsync<PagedOrdersDto>($"orders/all?offset={offset}&limit={limit}");
        if (pagedOrders?.Orders == null)
            throw new InvalidOperationException("Orders service unavailable");

        // Для каждого заказа достаём полные данные (чтобы получить id у items) и пользователя
        var userCache = new Dictionary<string, UserResponse>();

        async Task<OrderDetailsResponse> BuildDetailsAsync(OrderListItemDto brief)
        {
            // Детали заказа
            var orderDetailResp = await ordersClient.GetAsync($"orders/{brief.Id}");
            orderDetailResp.EnsureSuccessStatusCode();
            var fullOrder = await orderDetailResp.Content.ReadFromJsonAsync<OrderDto>();
            if (fullOrder == null) throw new InvalidOperationException($"Order {brief.Id} not found after fetch");

            // Пользователь
            UserResponse? user = null;
            if (!string.IsNullOrWhiteSpace(fullOrder.UserId))
            {
                if (!userCache.TryGetValue(fullOrder.UserId, out var cached))
                {
                    user = await FetchUserAsync(usersClient, fullOrder.UserId);
                    if (user != null) userCache[fullOrder.UserId] = user;
                }
                else
                {
                    user = cached;
                }
            }

            return new OrderDetailsResponse
            {
                Id = fullOrder.Id,
                UserId = fullOrder.UserId,
                Status = fullOrder.Status.ToLowerInvariant(),
                Total = fullOrder.Total,
                Items = fullOrder.Items.Select(i => new OrderItemWithIdResponse
                {
                    Id = i.Id,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                User = user,
                CreatedAt = fullOrder.CreatedAt,
                UpdatedAt = fullOrder.UpdatedAt
            };
        }

        var tasks = pagedOrders.Orders.Select(BuildDetailsAsync).ToArray();
        var orders = await Task.WhenAll(tasks);

        return new PagedOrdersResponse
        {
            Orders = orders.ToList(),
            Total = pagedOrders.Total
        };
    }

    public async Task<OrderDetailsResponse?> GetOrderByIdAsync(long id)
    {
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        using var orderResp = await ordersClient.GetAsync($"orders/{id}");
        if (orderResp.StatusCode == HttpStatusCode.NotFound)
            return null;

        orderResp.EnsureSuccessStatusCode();
        var order = await orderResp.Content.ReadFromJsonAsync<OrderDto>();
        if (order == null) return null;

        var user = await FetchUserAsync(usersClient, order.UserId);

        return new OrderDetailsResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            Status = order.Status.ToLowerInvariant(),
            Total = order.Total,
            Items = order.Items.Select(i => new OrderItemWithIdResponse
            {
                Id = i.Id,
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            User = user,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public async Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        var ordersClient = _httpClientFactory.CreateClient("orders");

        var createOrderDto = new CreateOrderDto
        {
            UserId = request.UserId,
            Status = "pending",
            Items = request.Items.Select(i => new CreateOrderItemDto
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        var response = await ordersClient.PostAsJsonAsync("orders", createOrderDto);
        response.EnsureSuccessStatusCode();

        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        if (createdOrder == null)
            throw new InvalidOperationException("Failed to create order");

        return new CreateOrderResponse
        {
            UserId = createdOrder.UserId,
            Status = createdOrder.Status.ToLowerInvariant(),
            Items = createdOrder.Items.Select(i => new OrderItemResponse
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }

    public async Task<OrderDetailsResponse?> UpdateOrderStatusAsync(long orderId, UpdateOrderStatusRequest request)
    {
        var paymentClient = _httpClientFactory.CreateClient("payments");
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        var updateDto = new UpdateOrderStatusDto { Status = request.Status };
        var response = await paymentClient.PostAsJsonAsync($"orders/{orderId}/status", updateDto);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
        response.EnsureSuccessStatusCode();

        var orderResponse = await ordersClient.GetAsync($"orders/{orderId}");
        if (orderResponse.StatusCode == HttpStatusCode.NotFound)
            return null;
        orderResponse.EnsureSuccessStatusCode();
        var updatedOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        if (updatedOrder == null)
            throw new InvalidOperationException("Failed to get updated order");

        var user = await FetchUserAsync(usersClient, updatedOrder.UserId);

        return new OrderDetailsResponse
        {
            Id = updatedOrder.Id,
            UserId = updatedOrder.UserId,
            Status = updatedOrder.Status.ToLowerInvariant(),
            Total = updatedOrder.Total,
            Items = updatedOrder.Items.Select(i => new OrderItemWithIdResponse
            {
                Id = i.Id,
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            User = user,
            CreatedAt = updatedOrder.CreatedAt,
            UpdatedAt = updatedOrder.UpdatedAt
        };
    }

    private static async Task<UserResponse?> FetchUserAsync(HttpClient usersClient, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return null;
        try
        {
            using var resp = await usersClient.GetAsync($"users/{userId}");
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<UserResponse>();
        }
        catch
        {
            return null; // не валим запрос, если сервис пользователей временно недоступен
        }
    }
}