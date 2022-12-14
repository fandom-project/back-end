using AutoMapper;
using Fandom_Project.Models;
using Fandom_Project.Models.DataTransferObjects.CommunityModel;
using Fandom_Project.Models.DataTransferObjects.PostModel;
using Fandom_Project.Models.DataTransferObjects.UserCommunityModel;
using Fandom_Project.Models.DataTransferObjects.UserModel;

namespace Fandom_Project.Models.DataTransferObjects
{
    // This will tell AutoMapper how to execute mapping actions
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mapping
            CreateMap<User, UserDto>();
            CreateMap<UserCreationDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<UserAuthenticationDto, User>();

            // Community mapping
            CreateMap<Community, CommunityDto>();
            CreateMap<CommunityUpdateDto, Community>();
            CreateMap<CommunityCreationDto, Community>();
            CreateMap<Community, CommunitySimpleDto>();

            // UserCommunity mapping
            CreateMap<UserCommunityUpdateDto, UserCommunity>();          
            CreateMap<UserCommunityCreateDto, UserCommunity>();
            CreateMap<UserCommunityDeleteDto, UserCommunity>();

            // Post mapping
            CreateMap<Post, PostDto>();
            CreateMap<PostCreateDto, Post>();
            CreateMap<Post, PostFollowDto>();
        }
    }
}
