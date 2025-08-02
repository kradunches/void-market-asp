using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Controller;
using PaymentService.Dto;
using Shared.Contracts;

namespace PaymentService.Controllers;

[ApiController]
[Route("payment/orders")]
public class PaymentController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOrderServiceClient _orderServiceClient;

    public PaymentController(IPublishEndpoint publishEndpoint, IOrderServiceClient orderServiceClient)
    {
        _publishEndpoint = publishEndpoint;
        _orderServiceClient = orderServiceClient;
    }

    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        if (!await _orderServiceClient.OrderExistsAsync(id)) // Использовать клиент
        {
            return NotFound($"Order with ID {id} not found");
        }

        await _publishEndpoint.Publish(new OrderStatusUpdated
        {
            OrderId = id,
            Status = request.Status
        });

        return Accepted(new { message = "status update published" });
    }
}