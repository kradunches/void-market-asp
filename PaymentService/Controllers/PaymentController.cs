using MassTransit;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Dto;
using Shared.Contracts;

namespace PaymentService.Controllers;

[ApiController]
[Route("orders")]
public class PaymentController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PaymentController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("{id:long}/status")]
    public IActionResult UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _ = _publishEndpoint.Publish(new OrderStatusUpdated
        {
            OrderId = id,
            Status = request.Status.ToString().ToLowerInvariant()
        });

        return Accepted();
    }
}