using System.ComponentModel.DataAnnotations;

namespace PaymentService.Dto;

public class UpdateOrderStatusRequestDto
{
    [Range(0, 4, ErrorMessage = "Status должен быть от 0 до 4 (Pending=0, Paid=1, Shipped=2, Delivery=3, Cancelled=4)")]
    public int Status { get; set; }
}