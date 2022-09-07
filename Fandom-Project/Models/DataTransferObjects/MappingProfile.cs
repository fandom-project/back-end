using AutoMapper;
using Fandom_Project.Models;
using Fandom_Project.Models.DataTransferObjects;

namespace Fandom_Project.Models.DataTransferObjects
{
    // This will tell AutoMapper how to execute mapping actions
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserCreationDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<UserAuthenticationDto, User>();
        }
    }
}
