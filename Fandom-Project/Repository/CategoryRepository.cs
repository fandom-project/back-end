using Fandom_Project.Data;
using Fandom_Project.Models;
using Fandom_Project.Repository.Interfaces;

namespace Fandom_Project.Repository
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(FandomContext fandomContext) : base(fandomContext) { }

        public void CreateCategory(Category category)
        {
            Create(category);
        }

        public void DeleteCategory(Category category)
        {
            Delete(category);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return FindAll().OrderBy(category => category.Name).ToList(); ;
        }

        public Category GetCategoryById(int id)
        {
            return FindByCondition(category => category.Equals(id)).FirstOrDefault();
        }

        public void UpdateCategory(Category category)
        {
            Update(category);
        }
    }
}
