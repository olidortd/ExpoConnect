using AutoMapper;
using ExpoConnect.Contracts.Users;
using ExpoConnect.Domain.Users;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ExpoConnect.Application.Mapping;

public class UsersProfile : Profile
{
    public UsersProfile()
    {
        CreateMap<User, UserResponse>()
            .ForMember(d => d.Role, m => m.MapFrom(s => s.Role.ToString()));
    }
}
