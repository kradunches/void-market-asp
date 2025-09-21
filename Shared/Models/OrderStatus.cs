using System.Runtime.Serialization;

namespace OrderService.Models;


public enum OrderStatus
{
    [EnumMember(Value = "pending")]
    Pending,
    [EnumMember(Value = "paid")]
    Paid,
    [EnumMember(Value = "shipped")]
    Shipped,
    [EnumMember(Value = "delivery")]
    Delivery,
    [EnumMember(Value = "cancelled")]
    Cancelled
}