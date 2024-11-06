using COCOApp.Models;
using COCOApp.Repositories;
using System.Collections.Generic;

namespace COCOApp.Services
{
    public class ProductService : StoreManagerService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetProducts()
        {
            return _productRepository.GetProducts();
        }

        public Product GetProductById(int productId, int sellerId)
        {
            return _productRepository.GetProductById(productId, sellerId);
        }

        public List<Product> GetProducts(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            return _productRepository.GetProducts(nameQuery, pageNumber, pageSize, sellerId, statusId);
        }

        public int GetTotalProducts(string nameQuery, int sellerId, int statusId)
        {
            return _productRepository.GetTotalProducts(nameQuery, sellerId, statusId);
        }

        public void AddProduct(Product product)
        {
            _productRepository.AddProduct(product);
        }

        public void EditProduct(int productId, Product product)
        {
            _productRepository.EditProduct(productId, product);
        }
    }
}
