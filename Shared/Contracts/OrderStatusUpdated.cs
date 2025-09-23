namespace Shared.Contracts;

public class OrderStatusUpdated
{
    public long OrderId { get; set; }
    public string Status { get; set; }
}