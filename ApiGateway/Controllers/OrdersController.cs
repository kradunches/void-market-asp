using ApiGateway.Models;
using ApiGateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderAggregateService _orderAggregateService;

    public OrdersController(IOrderAggregateService orderAggregateService)
    {
        _orderAggregateService = orderAggregateService;
    }

    [HttpGet("all")]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetAllOrders([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var result = await _orderAggregateService.GetAllOrdersAsync(offset, limit);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
    {
        var order = await _orderAggregateService.GetOrderByIdAsync(id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(CreateOrderRequest request)
    {
        var order = await _orderAggregateService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    [HttpPost("{id:int}/status")]
    public async Task<ActionResult<OrderResponse>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await _orderAggregateService.UpdateOrderStatusAsync(id, request);
        return Ok(order);
    }
}