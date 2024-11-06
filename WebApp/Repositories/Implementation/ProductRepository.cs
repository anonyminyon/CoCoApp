using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace COCOApp.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreManagerContext _context;

        public ProductRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<Product> GetProducts()
        {
            return _context.Products.AsQueryable().ToList();
        }

        public Product GetProductById(int productId, int sellerId)
        {
            var query = _context.Products.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            return productId > 0 ? query
                .Include(i => i.InventoryManagement)
                .Include(c => c.Category)
                .FirstOrDefault(u => u.Id == productId) : null;
        }

        public List<Product> GetProducts(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.Products.AsQueryable();
            bool status = statusId == 1;
            if (statusId > 0)
            {
                query = query.Where(p => p.Status == status);
            }
            if (sellerId > 0)
            {
                query = query.Where(p => p.SellerId == sellerId);
            }
            query = query
                .Include(i => i.InventoryManagement)
                .Include(c => c.Category);
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.ProductName.Contains(nameQuery)||c.Category.CategoryName.Contains(nameQuery));
            }
            query = query
                .OrderByDescending(p => p.Id);
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetTotalProducts(string nameQuery, int sellerId, int statusId)
        {
            var query = _context.Products.AsQueryable();
            bool status = statusId == 1;
            if (statusId > 0)
            {
                query = query.Where(p => p.Status == status);
            }
            if (sellerId > 0)
            {
                query = query.Where(p => p.SellerId == sellerId);
            }
            query = query.Include(i => i.InventoryManagement).Include(c => c.Category);
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.ProductName.Contains(nameQuery) || c.Category.CategoryName.Contains(nameQuery));
            }
            return query.Count();
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void EditProduct(int productId, Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(c => c.Id == productId);

            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.Cost = product.Cost;
                existingProduct.MeasureUnit = product.MeasureUnit;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Status = product.Status;
                existingProduct.SellerId = product.SellerId;
                existingProduct.UpdatedAt = product.UpdatedAt;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Product not found");
            }
        }
    }
}
