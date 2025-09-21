using Yarp.ReverseProxy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("orders", c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:OrderBaseAddress"]));
builder.Services.AddHttpClient("users", c =>
    c.BaseAddress = new Uri(builder.Configuration["Services:UserBaseAddress"]));

var app = builder.Build();

//хуячим minimal api прямо здесь чтобы не создавать контроллеры
// GET /api/orders/all?offset=&limit=
app.MapGet("/api/orders/all", async (int offset, int limit, IHttpClientFactory http) =>
{
    var ordersClient = http.CreateClient("orders");
    var usersClient = http.CreateClient("users");

    // получаем пагинированный список из OrderService
    var paged =
        await ordersClient.GetFromJsonAsync<PagedOrdersResponseDto>($"api/orders/all?offset={offset}&limit={limit}");
    if (paged == null) return Results.Problem("Orders service unavailable");

    // собираем уникальные userId и параллельно подтягиваем пользователей
    var userIds = paged.Orders.Select(o => o.UserId).Distinct().ToList();
    var userTasks = userIds.Select(async id =>
    {
        var u = await usersClient.GetFromJsonAsync<UserDto>($"/api/users/{id}");
        return (id, user: u);
    });
    var users = (await Task.WhenAll(userTasks)).Where(x => x.user != null).ToDictionary(x => x.id, x => x.user!);

    // клеим ответ
    var result = new
    {
        orders = paged.Orders.Select(o => new
        {
            id = o.Id,
            user = users.TryGetValue(o.UserId, out var u) ? new { id = u.Id, name = u.Name, email = u.Email } : null,
            status = o.Status.ToString().ToLowerInvariant(),
            total = o.Total,
            items = o.Items.Select(i => new { name = i.Name, quantity = i.Quantity, unitPrice = i.UnitPrice }),
            createdAt = o.CreatedAt,
            updatedAt = o.UpdatedAt
        }),
        total = paged.Total
    };

    return Results.Ok(result);
});

// GET /api/orders/{id}
app.MapGet("/api/orders/{id:int}", async (int id, IHttpClientFactory http) =>
{
    var ordersClient = http.CreateClient("orders");
    var usersClient = http.CreateClient("users");

    try
    {
        // FIX: корректный маршрут OrderService
        using var orderResp = await ordersClient.GetAsync($"/api/orders/{id}");
        if (orderResp.StatusCode == System.Net.HttpStatusCode.NotFound)
            return Results.NotFound();
        orderResp.EnsureSuccessStatusCode();

        var order = await orderResp.Content.ReadFromJsonAsync<OrderDto>();
        if (order == null) return Results.Problem("Empty order payload");

        UserDto? user = null;
        user = await usersClient.GetFromJsonAsync<UserDto>($"/api/users/{order.UserId}");

        var result = new
        {
            id = order.Id,
            user = user == null ? null : new { id = user.Id, name = user.Name, email = user.Email },
            status = order.Status.ToString().ToLowerInvariant(),
            items = order.Items.Select(i => new { name = i.Name, quantity = i.Quantity, unitPrice = i.UnitPrice }),
            createdAt = order.CreatedAt,
            updatedAt = order.UpdatedAt
        };

        return Results.Ok(result);
    }
    catch (HttpRequestException ex)
    {
        return Results.Problem($"Orders service error: {ex.Message}");
    }
});

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }

app.MapReverseProxy();

// app.MapOpenApi();
app.UseHttpsRedirection();

app.Run("http://0.0.0.0:80");

// DTOs used by aggregator
// задублировал эти dto чтобы не париться с генерацией клиентов через OpenAPI
public class UserDto { public string Id { get; set; } public string Name { get; set; } public string Email { get; set; } }
public class OrderItemBriefDto { public string Name { get; set; } public int Quantity { get; set; } public decimal UnitPrice { get; set; } }
public class OrderListItemDto { public int Id { get; set; } public string UserId { get; set; } public string Status { get; set; } public decimal Total { get; set; } public List<OrderItemBriefDto> Items { get; set; } public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } }
public class PagedOrdersResponseDto { public List<OrderListItemDto> Orders { get; set; } public int Total { get; set; } }
public class OrderItemDto { public int Id { get; set; } public string Name { get; set; } public int Quantity { get; set; } public decimal UnitPrice { get; set; } }
public class OrderDto { public int Id { get; set; } public string UserId { get; set; } public string Status { get; set; } public decimal Total { get; set; } public List<OrderItemDto> Items { get; set; } public DateTime CreatedAt { get; set; } public DateTime UpdatedAt { get; set; } }