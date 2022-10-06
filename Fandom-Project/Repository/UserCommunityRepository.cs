using Fandom_Project.Data;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;

namespace Fandom_Project.Repository
{
    public class UserCommunityRepository : RepositoryBase<UserCommunity>, IUserCommunityRepository
    {
        public UserCommunityRepository(FandomContext fandomContext) : base(fandomContext) { }

        public void AddUserToCommunity(UserCommunity userCommunity)
        {
            Create(userCommunity);
        }        

        public IEnumerable<UserCommunity> GetCommunitiesByUser(int userId)
        {
            return FindByCondition(community => community.UserId.Equals(userId)).ToList();
        }

        public IEnumerable<UserCommunity> GetUsersByCommunity(int communityId)
        {
            return FindByCondition(user => user.CommunityId.Equals(communityId)).ToList();
        }

        public void RemoveUserFromCommunity(UserCommunity userCommunity)
        {
            Delete(userCommunity);
        }

        public void UpdateUserRoleOnCommunity(UserCommunity userCommunity)
        {
            Update(userCommunity);
        }
    }
}
