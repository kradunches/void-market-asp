using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Controller;
using PaymentService.Dto;
using Shared.Contracts;

namespace PaymentService.Controllers;

[ApiController]
[Route("orders")]
public class PaymentController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IOrderServiceClient _orderServiceClient;

    public PaymentController(IPublishEndpoint publishEndpoint, IOrderServiceClient orderServiceClient)
    {
        _publishEndpoint = publishEndpoint;
        _orderServiceClient = orderServiceClient;
    }

    [HttpPost("{id:long}/status")]
    public async Task<IActionResult> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (!await _orderServiceClient.OrderExistsAsync(id))
            return NotFound($"Order with ID {id} not found");

        await _publishEndpoint.Publish(new OrderStatusUpdated
        {
            OrderId = id,
            Status = request.Status.ToString().ToLowerInvariant()
        });

        return Accepted(new { message = "status update published" });
    }
}