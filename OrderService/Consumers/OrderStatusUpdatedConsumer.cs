using AutoMapper;
using MassTransit;
using OrderService.Data;
using OrderService.Models;
using Shared.Contracts;

namespace OrderService.Consumers;

public class OrderStatusUpdatedConsumer : IConsumer<OrderStatusUpdated>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderStatusUpdatedConsumer(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Найти заказ и обновить статус
    public async Task Consume(ConsumeContext<OrderStatusUpdated> context)
    {
        var message = context.Message;

        var order = await _unitOfWork.Set<Order>().FindAsync(message.OrderId);

        if (order == null)
            return;
        
        order.Status = message.Status.ToLowerInvariant();
        order.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync();
    }
}