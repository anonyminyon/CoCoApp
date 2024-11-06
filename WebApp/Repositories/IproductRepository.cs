using COCOApp.Models;
using System.Collections.Generic;

namespace COCOApp.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetProducts();
        Product GetProductById(int productId, int sellerId);
        List<Product> GetProducts(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId);
        int GetTotalProducts(string nameQuery, int sellerId, int statusId);
        void AddProduct(Product product);
        void EditProduct(int productId, Product product);
    }
}
