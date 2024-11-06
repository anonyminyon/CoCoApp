using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace COCOApp.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly StoreManagerContext _context;

        public ReportRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<Report> GetReports(int customerId, int sellerId)
        {
            try
            {
                var query = _context.Reports.AsQueryable();
                if (sellerId > 0)
                {
                    query = query.Where(r => r.SellerId == sellerId);
                }
                query = query.Include(r => r.Customer)
                             .Where(r => r.CustomerId == customerId);

                return query.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving reports: {ex.Message}");
                throw new ApplicationException("Error retrieving reports", ex);
            }
        }
        public Report GetReportById(int reportId, int sellerId)
        {
            try
            {
                var query = _context.Reports.AsQueryable();
                if (sellerId > 0)
                {
                    query = query.Where(r => r.SellerId == sellerId);
                }
                query = query.Include(r => r.Customer);

                return query.FirstOrDefault(r => r.Id == reportId); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving reports: {ex.Message}");
                throw new ApplicationException("Error retrieving reports", ex);
            }
        }
        public List<ReportDetail> GetReportDetails(int reportId)
        {
            try
            {
                var query = _context.ReportDetails.AsQueryable();
                query = query.Include(p=>p.Product)
                             .Include(r => r.Report)
                             .ThenInclude(r => r.Customer)
                             .Where(r => r.ReportId == reportId);

                return query.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving reports: {ex.Message}");
                throw new ApplicationException("Error retrieving reports", ex);
            }
        }

        public void AddReport(Report report)
        {
            try
            {
                _context.Reports.Add(report);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding report: {ex.Message}");
                throw new ApplicationException("Error adding report", ex);
            }
        }

        public void AddReportDetails(ReportDetail reportDetail)
        {
            try
            {
                ReportDetail existedReportDetail=_context.ReportDetails.FirstOrDefault(rd=>rd.ReportId == reportDetail.ReportId&&rd.ProductId==reportDetail.ProductId);
                if (existedReportDetail != null)
                {
                    existedReportDetail.Volume += reportDetail.Volume; ;
                    existedReportDetail.TotalPrice+= reportDetail.TotalPrice; ; 
                }
                else
                {
                    _context.ReportDetails.Add(reportDetail);
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error adding report: {ex.Message}");
                throw new ApplicationException("Error adding report", ex);
            }
        }
    }
}
