namespace COCOApp.Models
{
    public class DashboardViewModel
    {
        public List<ProductStatistic> ProductStatistics { get; set; }
        public decimal TotalRevenue { get; set; }
        public string SelectedTimeRange { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
