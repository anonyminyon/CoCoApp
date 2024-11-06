using COCOApp.Models;
using COCOApp.Repositories;
using System.Collections.Generic;

namespace COCOApp.Services
{
    public class ReportService : StoreManagerService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public List<Report> GetReports(int customerId, int sellerId)
        {
            return _reportRepository.GetReports(customerId, sellerId);
        }
        public Report GetReportById(int reportId, int sellerId)
        {
            return _reportRepository.GetReportById(reportId, sellerId);
        }
        public List<ReportDetail> GetReportDetails(int reportId)
        {
            return _reportRepository.GetReportDetails(reportId);
        }

        public void AddReport(Report report)
        {
            _reportRepository.AddReport(report);
        }
        public void AddReportDetails(ReportDetail reportDetail)
        {
            _reportRepository.AddReportDetails(reportDetail);
        }
    }
}
