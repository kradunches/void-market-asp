using System.Net;
using ApiGateway.Models;

namespace ApiGateway.Services;

public interface IOrderAggregateService
{
    Task<PagedResponse<OrderResponse>> GetAllOrdersAsync(int offset, int limit);
    Task<OrderResponse?> GetOrderByIdAsync(int id);
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request);
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

    public async Task<PagedResponse<OrderResponse>> GetAllOrdersAsync(int offset, int limit)
    {
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        var pagedOrders = await ordersClient.GetFromJsonAsync<PagedOrdersDto>($"orders/all?offset={offset}&limit={limit}");
        if (pagedOrders?.Orders == null)
            throw new InvalidOperationException("Orders service unavailable");

        var userIds = pagedOrders.Orders.Select(o => o.UserId).Distinct().ToList();
        var users = await FetchUsersAsync(usersClient, userIds);

        var orderResponses = pagedOrders.Orders.Select(o => new OrderResponse
        {
            Id = o.Id,
            User = users.TryGetValue(o.UserId, out var user) ? user : null,
            Status = o.Status?.ToLowerInvariant(),
            Total = o.Total,
            Items = o.Items.Select(i => new OrderItemResponse
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            CreatedAt = o.CreatedAt,
            UpdatedAt = o.UpdatedAt
        }).ToList();

        return new PagedResponse<OrderResponse>
        {
            Data = orderResponses,
            Total = pagedOrders.Total
        };
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int id)
    {
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        using var orderResp = await ordersClient.GetAsync($"orders/{id}");
        if (orderResp.StatusCode == HttpStatusCode.NotFound)
            return null;

        orderResp.EnsureSuccessStatusCode();
        var order = await orderResp.Content.ReadFromJsonAsync<OrderDto>();
        if (order == null) return null;

        var users = await FetchUsersAsync(usersClient, [order.UserId]);

        return new OrderResponse
        {
            Id = order.Id,
            User = users.TryGetValue(order.UserId, out var user) ? user : null,
            Status = order.Status?.ToLowerInvariant(),
            Total = order.Total,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
    {
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        var createOrderDto = new CreateOrderDto
        {
            UserId = request.UserId,
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

        var users = await FetchUsersAsync(usersClient, [createdOrder.UserId]);

        return new OrderResponse
        {
            Id = createdOrder.Id,
            User = users.TryGetValue(createdOrder.UserId, out var user) ? user : null,
            Status = createdOrder.Status?.ToLowerInvariant(),
            Total = createdOrder.Total,
            Items = createdOrder.Items.Select(i => new OrderItemResponse
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            CreatedAt = createdOrder.CreatedAt,
            UpdatedAt = createdOrder.UpdatedAt
        };
    }

    public async Task<OrderResponse> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request)
    {
        // Используем PaymentService для обновления статуса
        var paymentClient = _httpClientFactory.CreateClient("payments");
        var ordersClient = _httpClientFactory.CreateClient("orders");
        var usersClient = _httpClientFactory.CreateClient("users");

        var updateDto = new UpdateOrderStatusDto { Status = request.Status };
    
        var response = await paymentClient.PostAsJsonAsync($"orders/{orderId}/status", updateDto);
        response.EnsureSuccessStatusCode();

        var orderResponse = await ordersClient.GetAsync($"orders/{orderId}");
        orderResponse.EnsureSuccessStatusCode();
    
        var updatedOrder = await orderResponse.Content.ReadFromJsonAsync<OrderDto>();
        if (updatedOrder == null)
            throw new InvalidOperationException("Failed to get updated order");

        var users = await FetchUsersAsync(usersClient, [updatedOrder.UserId]);

        return new OrderResponse
        {
            Id = updatedOrder.Id,
            User = users.TryGetValue(updatedOrder.UserId, out var user) ? user : null,
            Status = updatedOrder.Status?.ToLowerInvariant(),
            Total = updatedOrder.Total,
            Items = updatedOrder.Items.Select(i => new OrderItemResponse
            {
                Name = i.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList(),
            CreatedAt = updatedOrder.CreatedAt,
            UpdatedAt = updatedOrder.UpdatedAt
        };
    }


    private async Task<Dictionary<string, UserResponse>> FetchUsersAsync(HttpClient usersClient, List<string> userIds)
    {
        if (!userIds.Any()) return new Dictionary<string, UserResponse>();

        var userTasks = userIds.Select(async id =>
        {
            try
            {
                using var resp = await usersClient.GetAsync($"users/{id}");
                if (!resp.IsSuccessStatusCode)
                {
                    _logger.LogWarning("UserService returned {Status} for user {Id}", resp.StatusCode, id);
                    return (id, user: (UserDto?)null);
                }
                var userDto = await resp.Content.ReadFromJsonAsync<UserDto>();
                return (id, user: userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserService request failed for user {Id}", id);
                return (id, user: (UserDto?)null);
            }
        });

        var userResults = await Task.WhenAll(userTasks);
        
        return userResults
            .Where(x => x.user != null)
            .ToDictionary(
                x => x.id,
                x => new UserResponse
                {
                    Id = x.user!.Id,
                    Name = x.user.Name,
                    Email = x.user.Email
                });
    }
}