namespace Fandom_Project.Repository.Interfaces
{
    public interface IRepositoryWrapper
    {
        IUserRepository User { get; }
        ICategoryRepository Category { get; }
        void Save();
    }
}
