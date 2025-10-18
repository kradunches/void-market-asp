using OrderService.Models;

namespace OrderService.Dto;

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

public class OrderDtoCreate
{
    public string UserId { get; set; }
    public string Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderDtoUpdate
{
    public string UserId { get; set; }
    public string Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderItemBriefDto
{
    public string Name { get; set; }
    public long Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderListItemDto
{
    public long Id { get; set; }
    public string UserId { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemBriefDto> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PagedOrdersResponseDto
{
    public List<OrderListItemDto> Orders { get; set; }
    public int Total { get; set; }
}