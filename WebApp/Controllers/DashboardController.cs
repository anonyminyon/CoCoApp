using COCOApp.Models;
using COCOApp.Repositories;
using COCOApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace COCOApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ProductStatisticService _statisticService;

        public DashboardController(ProductStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        public async Task<IActionResult> Index(string timeRange = "day")
        {
            var endDate = DateTime.Now;
            var startDate = GetStartDate(endDate, timeRange);

            var viewModel = await _statisticService.GetDashboardDataAsync(startDate, endDate);
            viewModel.SelectedTimeRange = timeRange;
            viewModel.StartDate = startDate;
            viewModel.EndDate = endDate;

            return View(viewModel);
        }

        private DateTime GetStartDate(DateTime endDate, string timeRange)
        {
            return timeRange.ToLower() switch
            {
                "week" => endDate.AddDays(-7),
                "month" => endDate.AddMonths(-1),
                _ => endDate.Date // day
            };
        }
    }
}
