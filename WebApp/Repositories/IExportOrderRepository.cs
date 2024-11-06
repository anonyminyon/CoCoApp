using COCOApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace COCOApp.Repositories
{
    public interface IExportOrderRepository
    {
        List<ExportOrder> GetExportOrders();
        List<ExportOrder> GetExportOrdersByIds(List<int> orderIds, int sellerId);
        ExportOrder GetExportOrderById(int orderId, int sellerId);
        ExportOrder GetExportOrderByCustomerAndDate(int customerId, DateTime date);
        List<ExportOrder> GetExportOrders(string nameQuery, int pageNumber, int pageSize, int sellerId);
        int GetTotalExportOrders(string nameQuery, int sellerId);
        List<SelectListItem> GetCustomersSelectList(int sellerId);
        List<SelectListItem> GetProductsSelectList(int sellerId);
        void AddExportOrder(ExportOrder order);
        void EditExportOrder(int orderId, ExportOrder order);
        List<ExportOrder> GetExportOrders(string dateRange, int customerId, int sellerId);
    }
}
