using COCOApp.Models;

namespace COCOApp.Repositories
{
    public interface IImportOrderItemRepository
    {
        void addImportOrderItem(ImportOrderItem item);
        List<ImportOrderItem> GetImportOrderItems(int orderId, string nameQuery, int pageNumber, int pageSize, int sellerId);
        List<ImportOrderItem> GetImportOrderItems(int orderId, int sellerId);
        List<ImportOrderItem> GetImportOrderItemsByIds(List<int> orderIds, int sellerId);
        int GetTotalImportOrderItems(int orderId, string nameQuery, int sellerId);
        ImportOrderItem GetImportOrderItemById(int orderItemId, int productId, int sellerId);
        void EditImportOrderItem(int orderId, int productId, ImportOrderItem order);
    }
}
