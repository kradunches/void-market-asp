using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAggregateService _orderAggregateService;

    public OrdersController(IOrderAggregateService orderAggregateService)
    {
        _orderAggregateService = orderAggregateService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<PagedOrdersResponse>> GetAllOrders([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var result = await _orderAggregateService.GetAllOrdersAsync(offset, limit);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<OrderDetailsResponse>> GetOrderById(long id)
    {
        var order = await _orderAggregateService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        var order = await _orderAggregateService.CreateOrderAsync(request);
        // В ответе по контракту нет Id, поэтому в Location укажем коллекцию
        return CreatedAtAction(nameof(GetAllOrders), null, order);
    }

    [HttpPost("/api/payment/orders/{id:long}/status")]
    public async Task<IActionResult> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _orderAggregateService.UpdateOrderStatusAsync(id, request);
        if (order == null)
            return NotFound();
        return Ok();
    }
}