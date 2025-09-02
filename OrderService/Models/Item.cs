namespace OrderService.Models;

public class Item
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}