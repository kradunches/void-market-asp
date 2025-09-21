using System.Runtime.Serialization;

namespace OrderService.Models;


public enum OrderStatus
{
    pending,
    paid,
    shipped,
    delivery,
    cancelled
}