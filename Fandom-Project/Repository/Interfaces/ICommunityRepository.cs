using Fandom_Project.Models;

namespace Fandom_Project.Repository.Interfaces
{
    public interface ICommunityRepository : IRepositoryBase<Community>
    {
        IEnumerable<Community> GetAllCommunities();
        Community GetCommunityById(int id);
        Community GetCommunityBySlug(string slug);
        void CreateCommunity(Community community);
        void UpdateCommunity(Community community);
        void DeleteCommunity(Community community);        
    }
}
