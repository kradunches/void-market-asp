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
        
        if (Enum.IsDefined(typeof(OrderStatus), message.Status))
        {
            order.Status = (OrderStatus)message.Status;
            order.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }
    }
}