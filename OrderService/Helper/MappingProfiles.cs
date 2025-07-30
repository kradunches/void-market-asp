using AutoMapper;
using OrderService.Dto;
using OrderService.Models;

namespace OrderService.Helper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Order, OrderDto>().ReverseMap();
        CreateMap<Order, OrderDtoCreate>().ReverseMap();
        CreateMap<OrderItem, OrderItemDto>().ReverseMap();
    }
}