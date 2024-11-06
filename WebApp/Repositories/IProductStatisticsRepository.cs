using COCOApp.Models;

namespace COCOApp.Repositories
{
    public interface IProductStatisticsRepository
    {
        Task<List<ProductStatistic>> GetProductStatisticsAsync(DateTime startDate, DateTime endDate);
    }
}
