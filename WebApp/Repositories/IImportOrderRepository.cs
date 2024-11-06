using COCOApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace COCOApp.Repositories
{
    public interface IImportOrderRepository 
    {
        List<ImportOrder> GetImportOrders();
        List<ImportOrder> GetImportOrdersByIds(List<int> orderIds, int sellerId);
        ImportOrder GetImportOrderById(int orderId, int sellerId);
        ImportOrder GetImportOrderBySupplierAndDate(int customerId, DateTime date);
        List<ImportOrder> GetImportOrders(string nameQuery, int pageNumber, int pageSize, int sellerId);
        int GetTotalImportOrders(string nameQuery, int sellerId);
        List<SelectListItem> GetSuppliersSelectList(int sellerId);
        List<SelectListItem> GetProductsSelectList(int sellerId);
        void AddImportOrder(ImportOrder order);
        void EditImportOrder(int orderId, ImportOrder order);
        List<ImportOrder> GetImportOrders(string dateRange, int supplierId, int sellerId);
    }
}
