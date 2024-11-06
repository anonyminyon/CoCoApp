using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace COCOApp.Repositories.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly StoreManagerContext _context;

        public CategoryRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.AsQueryable().ToList();
        }

        public List<Category> GetCategories(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.Categories.AsQueryable();
            bool status = statusId == 1;
            if (statusId > 0)
            {
                query = query.Where(p => p.Status == status);
            }
            if (sellerId > 0)
            {
                query = query.Where(p => p.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.CategoryName.Contains(nameQuery));
            }
            query = query
                .OrderByDescending(p => p.Id);
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public Category GetCategoryById(int categoryId, int sellerId)
        {
            
            var query = _context.Categories.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            return categoryId > 0 ? query.FirstOrDefault(u => u.Id == categoryId) : null;

        }

        public void EditCategory(int categoryId, Category category)
        {
            var existingCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (existingCategory != null)
            {
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.Description = category.Description;
                existingCategory.Status = category.Status;
                existingCategory.SellerId = category.SellerId;
                existingCategory.UpdatedAt = category.UpdatedAt;
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Category not found");
            }
        }

        public int GetTotalCategories(string nameQuery, int sellerId, int statusId)
        {
            var query = _context.Categories.AsQueryable();
            bool status = statusId == 1;
            if (statusId > 0)
            {
                query = query.Where(p => p.Status == status);
            }
            if (sellerId > 0)
            {
                query = query.Where(p => p.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.CategoryName.Contains(nameQuery));
            }
            return query
                .Count();
        }

        public List<Category> GetCategories(int sellerId)
        {
            var query = _context.Categories.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(p => p.SellerId == sellerId);
            }
            return query.ToList();
        }
    }
}
