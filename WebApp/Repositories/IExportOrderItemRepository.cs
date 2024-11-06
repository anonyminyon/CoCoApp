using COCOApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace COCOApp.Repositories
{
    public interface IExportOrderItemRepository
    {
        void addExportOrderItem(ExportOrderItem item);
        List<ExportOrderItem> GetExportOrderItems(int orderId,string nameQuery, int pageNumber, int pageSize, int sellerId);
        List<ExportOrderItem> GetExportOrderItems(int orderId,int sellerId);
        List<ExportOrderItem> GetExportOrderItemsByIds(List<int> orderIds, int sellerId);
        int GetTotalExportOrderItems(int orderId,string nameQuery, int sellerId);
        ExportOrderItem GetExportOrderItemById(int orderItemId,int productId, int sellerId);
        void EditExportOrderItem(int orderId,int productId, ExportOrderItem order);
        List<ExportOrderItem> GetExportOrderItems(string dateRange, int customerId, int sellerId);
    }
}
