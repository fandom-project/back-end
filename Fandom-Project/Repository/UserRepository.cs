using Fandom_Project.Data;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;

namespace Fandom_Project.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(FandomContext fandomContext) : base(fandomContext) { }

        public void CreateUser(User user)
        {
            Create(user);
        }

        public void DeleteUser(User user)
        {
            Delete(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return FindAll().OrderBy(user => user.FullName).ToList();
        }

        public User GetUserById(int id)
        {
            return FindByCondition(user => user.UserId.Equals(id)).FirstOrDefault();
        }

        public bool ResetPassword(string email, string password)
        {
            var userResult = FindByCondition(user => user.Email.Equals(email)).FirstOrDefault();
            
            if (userResult == null)
            {
                return false;
            }

            userResult.Password = password;
            Update(userResult);
            return true;
        }

        public void UpdateUser(User user)
        {
            Update(user);
        }

        public bool UserAuthentication(string email, string password)
        {
            // TODO: Adjust this LINQ expression to only return email and password
            var userResult = FindByCondition(user => user.Email.Equals(email) && user.Password.Equals(password)).FirstOrDefault();
            if (userResult == null)
            {
                return false;
            }            
            return true;
        }
    }
}
