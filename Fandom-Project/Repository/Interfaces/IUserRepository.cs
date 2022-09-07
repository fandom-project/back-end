using Fandom_Project.Models;

namespace Fandom_Project.Repository.Interfaces
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        IEnumerable<User> GetAllUsers();
        User GetUserById(int id);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        bool UserAuthentication(string email, string password);
    }
}
