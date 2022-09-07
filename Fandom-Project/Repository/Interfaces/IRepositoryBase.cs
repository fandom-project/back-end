using System.Linq.Expressions;

namespace Fandom_Project.Repository.Interfaces
{
    public interface IRepositoryBase<T>
    {
        // Generic interface that will serve the project with all the CRUD methods
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
