using COCOApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace COCOApp.Repositories
{
    public class ExportOrderRepository : IExportOrderRepository
    {
        private readonly StoreManagerContext _context;

        public ExportOrderRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<ExportOrder> GetExportOrders()
        {
            return _context.ExportOrders
                              .Include(o => o.Customer)
                              .Include(o => o.ExportOrderItems)
                              .ThenInclude(oi => oi.Product)
                              .AsQueryable()
                              .ToList();
        }


        public List<ExportOrder> GetExportOrdersByIds(List<int> orderIds, int sellerId)
        {
            var query = _context.ExportOrders
                                  .Include(o => o.Customer)
                                  .Include(o => o.ExportOrderItems)
                                  .ThenInclude(oi => oi.Product)
                                .Where(o => orderIds.Contains(o.Id))
                                .AsQueryable();

            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }

            return query.ToList();
        }

        public ExportOrder GetExportOrderById(int orderId, int sellerId)
        {
            var query = _context.ExportOrders
                                  .Include(o => o.Customer)
                                  .Include(o => o.ExportOrderItems)
                                  .ThenInclude(oi => oi.Product)
                                .AsQueryable();

            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }

            return orderId > 0 ? query.FirstOrDefault(u => u.Id == orderId) : null;
        }

        public List<ExportOrder> GetExportOrders(string nameQuery, int pageNumber, int pageSize, int sellerId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.ExportOrders.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(o => o.Customer.Name.Contains(nameQuery));
            }
            query = query.Include(o => o.Customer)
                         .Include(o => o.ExportOrderItems)
                         .ThenInclude(oi => oi.Product)
                         .OrderByDescending(o => o.Id);

            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalExportOrders(string nameQuery, int sellerId)
        {
            var query = _context.ExportOrders.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Customer.Name.Contains(nameQuery));
            }
            query = query.Include(o => o.Customer)
                         .Include(o => o.ExportOrderItems)
                         .ThenInclude(oi => oi.Product);

            return query.Count();
        }

        public List<SelectListItem> GetCustomersSelectList(int sellerId)
        {
            var query = _context.Customers.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            return query.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }

        public List<SelectListItem> GetProductsSelectList(int sellerId)
        {
            var query = _context.Products.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(p => p.SellerId == sellerId);
            }
            return query.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.ProductName
            }).ToList();
        }

        public void AddExportOrder(ExportOrder order)
        {
            ExportOrder exportOrder=_context.ExportOrders.FirstOrDefault(o => o.CustomerId==order.CustomerId&&o.OrderDate==order.OrderDate);
            if (exportOrder == null)
            {
                _context.ExportOrders.Add(order);
                _context.SaveChanges();
            }
        }

        public void EditExportOrder(int orderId, ExportOrder order)
        {
            var existingOrder = _context.ExportOrders.FirstOrDefault(c => c.Id == orderId);

            if (existingOrder != null)
            {
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.UpdatedAt = order.UpdatedAt;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Order not found");
            }
        }

        public List<ExportOrder> GetExportOrders(string dateRange, int customerId, int sellerId)
        {
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MaxValue;

            if (!string.IsNullOrEmpty(dateRange))
            {
                var dateRangeParts = dateRange.Split(" - ");
                if (dateRangeParts.Length == 2)
                {
                    if (!DateTime.TryParse(dateRangeParts[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
                    {
                        startDate = DateTime.MinValue;
                    }
                    if (!DateTime.TryParse(dateRangeParts[1], CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
                    {
                        endDate = DateTime.MaxValue;
                    }
                }
            }

            try
            {
                var query = _context.ExportOrders.AsQueryable();
                if (sellerId > 0)
                {
                    query = query.Where(o => o.SellerId == sellerId);
                }
                query = query.Include(o => o.Customer)
                             .Include(o => o.ExportOrderItems)
                             .ThenInclude(oi => oi.Product)
                             .Where(o => o.CustomerId == customerId && o.OrderDate >= startDate && o.OrderDate <= endDate)
                             .OrderByDescending(o=>o.OrderDate);

                return query.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving orders: {ex.Message}");
                throw new ApplicationException("Error retrieving orders", ex);
            }
        }
        public ExportOrder GetExportOrderByCustomerAndDate(int customerId, DateTime date)
        {
            var query = _context.ExportOrders
                      .Include(o => o.Customer)
                      .Include(o => o.ExportOrderItems)
                      .ThenInclude(oi => oi.Product)
                    .AsQueryable();

            return query.OrderByDescending(o=>o.UpdatedAt)
                .FirstOrDefault(o=>o.CustomerId==customerId&&o.OrderDate==date);
        }
    }
}
