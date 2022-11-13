using Fandom_Project.Data;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;

namespace Fandom_Project.Repository
{
    public class PostRepository : RepositoryBase<Post>, IPostRepository
    {
        public PostRepository(FandomContext fandomContext) : base(fandomContext) { }

        public void AddPostToCommunity(Post post)
        {
            Create(post);
        }        

        public IEnumerable<Post> GetPostsByCommunity(int communityId)
        {
            return FindByCondition(post => post.CommunityId.Equals(communityId)).ToList();
        }        

        public void RemovePostFromCommunity(Post post)
        {
            Delete(post);
        }

        public void UpdatePostOnCommunity(Post post)
        {
            Update(post);
        }
    }
}
