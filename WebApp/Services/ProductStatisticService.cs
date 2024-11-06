using COCOApp.Models;
using COCOApp.Repositories;
using COCOApp.Repositories.Implementation;

namespace COCOApp.Services
{
    public class ProductStatisticService : StoreManagerService
    {
        private readonly IProductStatisticsRepository _repository;

        public ProductStatisticService(IProductStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(DateTime startDate, DateTime endDate)
        {
            var statistics = await _repository.GetProductStatisticsAsync(startDate, endDate);

            return new DashboardViewModel
            {
                ProductStatistics = statistics,
                TotalRevenue = statistics.Sum(s => s.Revenue)
            };
        }
    }
}
