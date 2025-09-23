using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Dto;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<PagedOrdersResponseDto> GetPagedOrdersAsync(int offset, int limit);
    Task<OrderDto?> GetOrderByIdAsync(long id);
    Task<OrderDto> CreateOrderAsync(OrderDtoCreate orderDtoCreate);
    Task<OrderDto?> UpdateOrderAsync(long id, OrderDtoUpdate orderDtoUpdate);
    Task<bool> DeleteOrderAsync(long id);
}

public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(ILogger<OrderService> logger, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedOrdersResponseDto> GetPagedOrdersAsync(int offset, int limit)
    {
        if (offset < 0) offset = 0;
        if (limit <= 0) limit = 10;
        if (limit > 100) limit = 100;

        var query = _unitOfWork.Set<Order>()
            .AsNoTracking()
            .Include(o => o.Items)
            .OrderBy(o => o.Id);

        var total = await query.CountAsync();
        var page = await query.Skip(offset).Take(limit).ToListAsync();

        return new PagedOrdersResponseDto
        {
            Orders = _mapper.Map<List<OrderListItemDto>>(page),
            Total = total
        };
    }

    public async Task<OrderDto?> GetOrderByIdAsync(long id)
    {
        async Task<bool> isOrderNotExistsAsync()
        {
            var isOrderExists = await _unitOfWork.Set<Order>().AnyAsync(o => o.Id == id);
            return !isOrderExists;
        }

        if (await isOrderNotExistsAsync())
        {
            string exMessage = $"No order exists with ID: {id}";
            _logger.LogInformation(exMessage);
            throw new InvalidOperationException(exMessage);
        }

        var orderEntity = await _unitOfWork.Set<Order>()
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id);
        var orderDto = _mapper.Map<OrderDto>(orderEntity);

        return orderDto;
    }

    public async Task<OrderDto> CreateOrderAsync(OrderDtoCreate orderDtoCreate)
    {
        var orderEntity = _mapper.Map<Order>(orderDtoCreate);

        var now = DateTime.UtcNow;

        orderEntity.CreatedAt = now;
        orderEntity.UpdatedAt = now;
        
        foreach (var i in orderEntity.Items)
        {
            i.CreatedAt = now;
            i.UpdatedAt = now;
        }
        
        orderEntity.Total = orderEntity.Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;

        await _unitOfWork.Set<Order>().AddAsync(orderEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderDto>(orderEntity);
    }

    public async Task<OrderDto?> UpdateOrderAsync(long id, OrderDtoUpdate orderDtoUpdate)
    {
        var orderEntity = await _unitOfWork.Set<Order>()
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id);

        if (orderEntity == null)
        {
            _logger.LogInformation("No order exists with ID: {OrderId}", id);
            return null;
        }
        
        var now = DateTime.UtcNow;

        var newItems = _mapper.Map<List<Item>>(orderDtoUpdate.Items ?? new List<OrderItemDto>());
        
        foreach (var i in newItems)
        {
            i.CreatedAt = now;
            i.UpdatedAt = now;
        }

        orderEntity.Items = newItems;

        orderEntity.Total = orderEntity.Items?.Sum(i => i.Quantity * i.UnitPrice) ?? 0;
        orderEntity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderDto>(orderEntity);
    }

    public async Task<bool> DeleteOrderAsync(long id)
    {
        var orderEntity = await _unitOfWork.Set<Order>()
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id);

        if (orderEntity == null)
        {
            _logger.LogInformation("No order exists with ID: {OrderId}", id);
            return false;
        }

        if (orderEntity.Items is { Count: > 0 })
            _unitOfWork.Set<Item>().RemoveRange(orderEntity.Items);

        _unitOfWork.Set<Order>().Remove(orderEntity);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}