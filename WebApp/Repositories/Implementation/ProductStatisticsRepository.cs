using COCOApp.Models;
using Microsoft.EntityFrameworkCore;

namespace COCOApp.Repositories.Implementation
{
    public class ProductStatisticsRepository : IProductStatisticsRepository
    {
        private readonly StoreManagerContext _context;

        public ProductStatisticsRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public async Task<List<ProductStatistic>> GetProductStatisticsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.ExportOrderItems
                .Where(eoi => eoi.Order.OrderDate >= startDate && eoi.Order.OrderDate <= endDate)
                .GroupBy(eoi => eoi.Product.ProductName)
                .Select(g => new ProductStatistic
                {
                    ProductName = g.Key,
                    Quantity = g.Sum(eoi => eoi.Volume),
                    Revenue = g.Sum(eoi => eoi.Total)
                })
                .ToListAsync();
        }
    }
}
