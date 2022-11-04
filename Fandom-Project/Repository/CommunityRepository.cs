using Fandom_Project.Data;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;

namespace Fandom_Project.Repository
{
    public class CommunityRepository : RepositoryBase<Community>, ICommunityRepository
    {
        public CommunityRepository(FandomContext fandomContext) : base(fandomContext) { }

        public void CreateCommunity(Community community)
        {
            Create(community);
        }

        public void DeleteCommunity(Community community)
        {
            Delete(community);
        }

        public IEnumerable<Community> GetAllCommunities()
        {
            return FindAll().OrderBy(community => community.Name).ToList();
        }

        public Community GetCommunityById(int id)
        {
            return FindByCondition(community => community.CommunityId.Equals(id)).FirstOrDefault();
        }   
        
        public Community GetCommunityBySlug(string slug)
        {
            return FindByCondition(community => community.Slug.Equals(slug)).FirstOrDefault();
        }

        public void UpdateCommunity(Community community)
        {
            Update(community);
        }        
    }
}
