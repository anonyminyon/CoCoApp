using COCOApp.Models;
using COCOApp.Repositories;

namespace COCOApp.Services
{
    public class CategoryService : StoreManagerService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public List<Category> GetCategories()
        {
            return _categoryRepository.GetCategories();
        }
        public List<Category> GetCategories(int sellerId)
        {
            return _categoryRepository.GetCategories(sellerId);
        }

        public List<Category> GetCategories(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            return _categoryRepository.GetCategories(nameQuery, pageNumber, pageSize, sellerId, statusId);
        }

        public Category GetCategoryById(int categoryId, int sellerId)
        {
            return _categoryRepository.GetCategoryById(categoryId, sellerId);
        }

        public void AddCategory(Category category)
        {
            _categoryRepository.AddCategory(category);
        }

        public void EditCategory(int categoryId, Category category)
        {
            _categoryRepository.EditCategory(categoryId, category);
        }

        public int GetTotalCategories(string nameQuery, int sellerId, int statusId)
        {
            return _categoryRepository.GetTotalCategories(nameQuery, sellerId, statusId);
        }
    }
}
