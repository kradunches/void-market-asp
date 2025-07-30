using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Dto;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDto> CreateOrderAsync(OrderDtoCreate orderDtoCreate);
    Task<bool> DeleteOrderAsync(int id);
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

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        var orderEntities = await _unitOfWork.Set<Order>()
            .Include(o => o.Items)
            .ToListAsync();
        
        var orderDtos = _mapper.Map<List<OrderDto>>(orderEntities);

        return orderDtos;
    }

    public async Task<OrderDto?> GetOrderByIdAsync(int id)
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
        await _unitOfWork.Set<Order>().AddAsync(orderEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<OrderDto>(orderEntity);
    }

    public async Task<bool> DeleteOrderAsync(int id)
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
            return false;
        }

        var orderEntity = await _unitOfWork.Set<Order>()
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == id);
        _unitOfWork.Set<Order>().Remove(orderEntity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}