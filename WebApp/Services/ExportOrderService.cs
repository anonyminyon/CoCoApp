using COCOApp.Models;
using COCOApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace COCOApp.Services
{
    public class ExportOrderService : StoreManagerService
    {
        private readonly IExportOrderRepository _orderRepository;

        public ExportOrderService(IExportOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<ExportOrder> GetExportOrders()
        {
            return _orderRepository.GetExportOrders();
        }

        public List<ExportOrder> GetExportOrdersByIds(List<int> orderIds, int sellerId)
        {
            return _orderRepository.GetExportOrdersByIds(orderIds, sellerId);
        }

        public ExportOrder GetExportOrderById(int orderId, int sellerId)
        {
            return _orderRepository.GetExportOrderById(orderId, sellerId);
        }

        public ExportOrder GetExportOrderByCustomerAndDate(int customerId,DateTime date)
        {
            return _orderRepository.GetExportOrderByCustomerAndDate(customerId, date);
        }

        public List<ExportOrder> GetExportOrders(string nameQuery, int pageNumber, int pageSize, int sellerId)
        {
            return _orderRepository.GetExportOrders(nameQuery, pageNumber, pageSize, sellerId);
        }

        public int GetTotalExportOrders(string nameQuery, int sellerId)
        {
            return _orderRepository.GetTotalExportOrders(nameQuery, sellerId);
        }

        public List<SelectListItem> GetCustomersSelectList(int sellerId)
        {
            return _orderRepository.GetCustomersSelectList(sellerId);
        }

        public List<SelectListItem> GetProductsSelectList(int sellerId)
        {
            return _orderRepository.GetProductsSelectList(sellerId);
        }

        public void AddExportOrder(ExportOrder order)
        {
            _orderRepository.AddExportOrder(order);
        }

        public void EditExportOrder(int orderId, ExportOrder order)
        {
            _orderRepository.EditExportOrder(orderId, order);
        }

        public List<ExportOrder> GetExportOrders(string dateRange, int customerId, int sellerId)
        {
            return _orderRepository.GetExportOrders(dateRange, customerId, sellerId);
        }
    }
}
