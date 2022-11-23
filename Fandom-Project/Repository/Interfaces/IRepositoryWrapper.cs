using Microsoft.EntityFrameworkCore.Storage;

namespace Fandom_Project.Repository.Interfaces
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        ICategoryRepository Category { get; }
        ICommunityRepository Community { get; }
        IUserCommunityRepository UserCommunity { get; }        
        IPostRepository Post { get; }        
        void Save();
        IDbContextTransaction BeginTransaction();
    }
}
