using AutoMapper;
using UserService.Models;

namespace UserService.Helper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString())).ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
    }
}