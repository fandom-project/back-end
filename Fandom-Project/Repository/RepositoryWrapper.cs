using Fandom_Project.Data;
using Fandom_Project.Repository.Interfaces;

namespace Fandom_Project.Repository
{
    // Class that holds all repository classes in one place so we don't need to call them individualy
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private FandomContext _fandomContext;
        private IUserRepository _user;
        private ICategoryRepository _category;

        public IUserRepository User
        {
            get
            {
                if(_user == null )
                {
                    _user = new UserRepository(_fandomContext);
                }
                return _user;
            }            
        }

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_fandomContext);
                }
                return _category;
            }
        }

        public RepositoryWrapper(FandomContext fandomContext)
        {
            _fandomContext = fandomContext;
        }

        // With this method we can do multiple operations and save all in the database at the same time
        // Success -> All changes will be applied | Fail -> All changes will be reverted
        public void Save()
        {
            _fandomContext.SaveChanges();
        }
    }
}
