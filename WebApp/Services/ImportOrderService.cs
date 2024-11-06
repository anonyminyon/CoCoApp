using COCOApp.Models;
using COCOApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace COCOApp.Services
{
    public class ImportOrderService : StoreManagerService
    {
        private readonly IImportOrderRepository _orderRepository;

        public ImportOrderService(IImportOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<ImportOrder> GetImportOrders()
        {
            return _orderRepository.GetImportOrders();
        }

        public List<ImportOrder> GetImportOrdersByIds(List<int> orderIds, int sellerId)
        {
            return _orderRepository.GetImportOrdersByIds(orderIds, sellerId);
        }

        public ImportOrder GetImportOrderById(int orderId, int sellerId)
        {
            return _orderRepository.GetImportOrderById(orderId, sellerId);
        }

        public ImportOrder GetImportOrderByCustomerAndDate(int supplierId, DateTime date)
        {
            return _orderRepository.GetImportOrderBySupplierAndDate(supplierId, date);
        }

        public List<ImportOrder> GetImportOrders(string nameQuery, int pageNumber, int pageSize, int sellerId)
        {
            return _orderRepository.GetImportOrders(nameQuery, pageNumber, pageSize, sellerId);
        }

        public int GetTotalImportOrders(string nameQuery, int sellerId)
        {
            return _orderRepository.GetTotalImportOrders(nameQuery, sellerId);
        }

        public List<SelectListItem> GetSuppliersSelectList(int sellerId)
        {
            return _orderRepository.GetSuppliersSelectList(sellerId);
        }

        public List<SelectListItem> GetProductsSelectList(int sellerId)
        {
            return _orderRepository.GetProductsSelectList(sellerId);
        }

        public void AddImportOrder(ImportOrder order)
        {
            _orderRepository.AddImportOrder(order);
        }

        public void EditImportOrder(int orderId, ImportOrder order)
        {
            _orderRepository.EditImportOrder(orderId, order);
        }

        public List<ImportOrder> GetImportOrders(string dateRange, int supplierId, int sellerId)
        {
            return _orderRepository.GetImportOrders(dateRange, supplierId, sellerId);
        }
    }
}
