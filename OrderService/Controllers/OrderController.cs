using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Dto;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("all")]
    [ProducesResponseType(200, Type = typeof(PagedOrdersResponseDto))]
    public async Task<ActionResult<PagedOrdersResponseDto>> GetAllPagedAsync([FromQuery] int offset = 0,
        [FromQuery] int limit = 10)
    {
        if (offset < 0 || limit <= 0) 
            return BadRequest("Invalid pagination parameters.");

        var result = await _orderService.GetPagedOrdersAsync(offset, limit);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(200, Type = typeof(OrderDto))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderDto>> GetOrderByIdAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var orderDto = await _orderService.GetOrderByIdAsync(id);
            return Ok(orderDto);
        }
        catch (InvalidOperationException)
        {
            return NotFound($"Order with Id: {id} not found");
        }
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(OrderDto))]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrderDto>> CreateOrderAsync([FromBody] OrderDtoCreate orderDtoCreate)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var createdOrderDto = await _orderService.CreateOrderAsync(orderDtoCreate);

        return Created($"/api/orders/{createdOrderDto.Id}", createdOrderDto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(200, Type = typeof(OrderDto))]
    [ProducesResponseType(404)]
    public async Task<ActionResult<OrderDto>> UpdateOrderAsync(int id, [FromBody] OrderDtoUpdate orderDtoUpdate)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var updatedOrderDto = await _orderService.UpdateOrderAsync(id, orderDtoUpdate);
        if (updatedOrderDto == null)
            return NotFound($"Order with Id: {id} not found");

        return Ok(updatedOrderDto);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteOrderAsync(int id)
    {
        var isDeleted = await _orderService.DeleteOrderAsync(id);

        if (!isDeleted)
        {
            return NotFound($"Order with Id: {id} not found");
        }

        return NoContent();
    }
}
