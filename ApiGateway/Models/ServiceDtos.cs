namespace ApiGateway.Models;

public class OrderItemDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderDto
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderListItemDto
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemBriefDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderItemBriefDto
{
    public string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class PagedOrdersDto
{
    public List<OrderListItemDto> Orders { get; set; } = new();
    public int Total { get; set; }
}

public class CreateOrderItemDto
{
    public string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CreateOrderDto
{
    public string UserId { get; set; }
    public string Status { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class UpdateOrderStatusDto
{
    public string Status { get; set; }
}