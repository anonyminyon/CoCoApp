using COCOApp.Models;
using COCOApp.Repositories;

namespace COCOApp.Services
{
    public class ImportOrderItemService : StoreManagerService
    {
        private readonly IImportOrderItemRepository _orderItemRepository;

        public ImportOrderItemService(IImportOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }
        public void AddImportOrderItem(ImportOrderItem order)
        {
            _orderItemRepository.addImportOrderItem(order);
        }
        public List<ImportOrderItem> GetImportOrderItems(int orderId,string nameQuery, int pageNumber, int pageSize, int sellerId)
        {
            return _orderItemRepository.GetImportOrderItems(orderId,nameQuery, pageNumber, pageSize, sellerId);
        }
        public List<ImportOrderItem> GetImportOrderItems(int orderId, int sellerId)
        {
            return _orderItemRepository.GetImportOrderItems(orderId,sellerId);
        }
        public List<ImportOrderItem> GetImportOrderItemsByIds(List<int> orderIds, int sellerId)
        {
            return _orderItemRepository.GetImportOrderItemsByIds(orderIds, sellerId);
        }

        public int GetTotalImportOrderItems(int orderId, string nameQuery, int sellerId)
        {
            return _orderItemRepository.GetTotalImportOrderItems(orderId,nameQuery, sellerId);
        }
        public ImportOrderItem GetImportOrderitemById(int orderId,int productId, int sellerId)
        {
            return _orderItemRepository.GetImportOrderItemById(orderId,productId, sellerId);
        }
        public void EditImportOrderItem(int orderId,int productId, ImportOrderItem order)
        {
            _orderItemRepository.EditImportOrderItem(orderId,productId, order);
        }
    }
}
