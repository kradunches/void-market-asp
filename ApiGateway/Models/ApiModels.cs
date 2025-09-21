namespace ApiGateway.Models;

public class UserResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class OrderItemResponse
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderResponse
{
    public int Id { get; set; }
    public UserResponse? User { get; set; }
    public string? Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
}

public class CreateOrderItemRequest
{
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderRequest
{
    public string UserId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class UpdateOrderStatusRequest
{
    public string Status { get; set; }
}