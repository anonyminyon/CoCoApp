using COCOApp.Models;

namespace COCOApp.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        List<Category> GetCategories(int sellerId);
        List<Category> GetCategories(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId);
        Category GetCategoryById(int categoryId, int sellerId);
        void AddCategory(Category category);
        void EditCategory(int categoryId, Category category);
        int GetTotalCategories(string nameQuery, int sellerId, int statusId);
    }
}
