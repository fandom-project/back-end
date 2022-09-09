using Fandom_Project.Data;
using Fandom_Project.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Fandom_Project.Repository
{
    // https://docs.microsoft.com/pt-br/dotnet/csharp/language-reference/keywords/abstract
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected FandomContext FandomContext { get; set; }
        public RepositoryBase(FandomContext fandomContext)
        {
            FandomContext = fandomContext;
        }

        public IQueryable<T> FindAll() => FandomContext.Set<T>().AsNoTracking();
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => FandomContext.Set<T>().Where(expression).AsNoTracking();        
        public void Create(T entity) => FandomContext.Set<T>().Add(entity);
        public void Update(T entity) => FandomContext.Set<T>().Update(entity);
        public void Delete(T entity) => FandomContext.Set<T>().Remove(entity);        
    }
}
