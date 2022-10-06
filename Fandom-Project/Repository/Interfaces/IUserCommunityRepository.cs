using Fandom_Project.Models;

namespace Fandom_Project.Repository.Interfaces
{
    public interface IUserCommunityRepository : IRepositoryBase<UserCommunity>
    {        
        IEnumerable<UserCommunity> GetCommunitiesByUser(int userId);
        IEnumerable<UserCommunity> GetUsersByCommunity(int communityId);
        void AddUserToCommunity(UserCommunity userCommunity);
        void UpdateUserRoleOnCommunity(UserCommunity userCommunity);
        void RemoveUserFromCommunity(UserCommunity userCommunity);        
    }
}
