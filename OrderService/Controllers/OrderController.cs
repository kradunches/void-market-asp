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

    [HttpGet("GetOrders")]
    [ProducesResponseType(200, Type = typeof(IEnumerable<OrderDto>))]
    public async Task<IResult> GetAllOrdersAsync()
    {
        var orderDtos = await _orderService.GetAllOrdersAsync();

        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        return TypedResults.Ok(orderDtos);
    }

    [HttpGet("GetOrder/{id}")]
    [ProducesResponseType(200, Type = typeof(OrderDto))]
    [ProducesResponseType(404)]
    public async Task<IResult> GetOrderByIdAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }

        try
        {
            var orderDto = await _orderService.GetOrderByIdAsync(id);
            return TypedResults.Ok(orderDto);
        }
        catch (InvalidOperationException)
        {
            return Results.NotFound($"Order with Id: {id} not found");
        }
    }

    [HttpPost("CreateOrder")]
    [ProducesResponseType(201, Type = typeof(OrderDto))]
    [ProducesResponseType(400)]
    public async Task<IResult> CreateOrderAsync([FromBody] OrderDtoCreate orderDtoCreate)
    {
        if (!ModelState.IsValid)
        {
            return Results.BadRequest(ModelState);
        }
        
        var createdOrderDto = await _orderService.CreateOrderAsync(orderDtoCreate);

        return TypedResults.Created($"/api/{ControllerContext.ActionDescriptor.ControllerName}/{createdOrderDto.Id}",
            createdOrderDto);
    }

    [HttpDelete("DeleteOrder/{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IResult> DeleteOrderAsync(int id)
    {
        var isDeleted = await _orderService.DeleteOrderAsync(id);

        if (!isDeleted)
        {
            return Results.NotFound($"Order with Id: {id} not found");
        }

        return TypedResults.NoContent();
    }
}
