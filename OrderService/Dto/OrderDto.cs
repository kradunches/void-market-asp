using OrderService.Models;

namespace OrderService.Dto;

public class OrderDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderDtoCreate
{
    public string UserId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDto> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}