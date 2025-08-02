namespace Shared.Contracts;

public class OrderStatusUpdated
{
    public int OrderId { get; set; }
    public int Status { get; set; }
}