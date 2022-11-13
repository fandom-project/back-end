using Fandom_Project.Models;

namespace Fandom_Project.Repository.Interfaces
{
    public interface IPostRepository : IRepositoryBase<Post>
    {        
        IEnumerable<Post> GetPostsByCommunity(int communityId);        
        void AddPostToCommunity(Post post);
        void UpdatePostOnCommunity(Post post);
        void RemovePostFromCommunity(Post post);        
    }
}
