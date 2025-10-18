using System.Text.Json.Serialization;

namespace ApiGateway.Models;

public class OrderItemResponse
{
    public required string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderItemWithIdResponse
{
    public required string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? CreatedAt { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTime? UpdatedAt { get; set; }
}

public class CreateOrderResponse
{
    public required string UserId { get; set; }
    public required string Status { get; set; }
    public List<OrderItemResponse> Items { get; set; } = new();
}

public class OrderDetailsResponse
{
    public long Id { get; set; }
    public required string UserId { get; set; }
    public required string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemBriefDto> Items { get; set; } = new();
    public UserResponse? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PagedOrdersResponse
{
    public List<OrderDetailsResponse> Orders { get; set; } = new();
    public int Total { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int Total { get; set; }
}

public class CreateOrderItemRequest
{
    public required string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderRequest
{
    public required string UserId { get; set; }
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class UpdateOrderStatusRequest
{
    public required string Status { get; set; }
}