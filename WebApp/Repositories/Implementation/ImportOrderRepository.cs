using COCOApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;

namespace COCOApp.Repositories
{
    public class ImportOrderRepository : IImportOrderRepository
    {
        private readonly StoreManagerContext _context;

        public ImportOrderRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<ImportOrder> GetImportOrders()
        {
            return _context.ImportOrders
                              .Include(o => o.Supplier)
                              .Include(o => o.ImportOrderItems)
                              .ThenInclude(oi => oi.Product)
                              .AsQueryable()
                              .ToList();
        }


        public List<ImportOrder> GetImportOrdersByIds(List<int> orderIds, int sellerId)
        {
            var query = _context.ImportOrders
                                  .Include(o => o.Supplier)
                                  .Include(o => o.ImportOrderItems)
                                  .ThenInclude(oi => oi.Product)
                                .Where(o => orderIds.Contains(o.Id))
                                .AsQueryable();

            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }

            return query.ToList();
        }

        public ImportOrder GetImportOrderById(int orderId, int sellerId)
        {
            var query = _context.ImportOrders
                                  .Include(o => o.Supplier)
                                  .Include(o => o.ImportOrderItems)
                                  .ThenInclude(oi => oi.Product)
                                .AsQueryable();

            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }

            return orderId > 0 ? query.FirstOrDefault(u => u.Id == orderId) : null;
        }

        public List<ImportOrder> GetImportOrders(string nameQuery, int pageNumber, int pageSize, int sellerId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.ImportOrders.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(o => o.Supplier.Name.Contains(nameQuery));
            }
            query = query.Include(o => o.Supplier)
                         .Include(o => o.ImportOrderItems)
                         .ThenInclude(oi => oi.Product)
                         .OrderByDescending(o => o.Id);

            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }

        public int GetTotalImportOrders(string nameQuery, int sellerId)
        {
            var query = _context.ImportOrders.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(o => o.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Supplier.Name.Contains(nameQuery));
            }
            query = query.Include(o => o.Supplier)
                         .Include(o => o.ImportOrderItems)
                         .ThenInclude(oi => oi.Product);

            return query.Count();
        }

        public List<SelectListItem> GetSuppliersSelectList(int sellerId)
        {
            var query = _context.Suppliers.AsQueryable();
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

        public void AddImportOrder(ImportOrder order)
        {
            ImportOrder importOrder = _context.ImportOrders.FirstOrDefault(o => o.SupplierId == order.SupplierId && o.OrderDate == order.OrderDate);
            if (importOrder == null)
            {
                _context.ImportOrders.Add(order);
                _context.SaveChanges();
            }
        }

        public void EditImportOrder(int orderId, ImportOrder order)
        {
            var existingOrder = _context.ImportOrders.FirstOrDefault(c => c.Id == orderId);

            if (existingOrder != null)
            {
                existingOrder.SupplierId = order.SupplierId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.UpdatedAt = order.UpdatedAt;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Order not found");
            }
        }

        public List<ImportOrder> GetImportOrders(string dateRange, int supplierId, int sellerId)
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
                var query = _context.ImportOrders.AsQueryable();
                if (sellerId > 0)
                {
                    query = query.Where(o => o.SellerId == sellerId);
                }
                query = query.Include(o => o.Supplier)
                             .Include(o => o.ImportOrderItems)
                             .ThenInclude(oi => oi.Product)
                             .Where(o => o.SupplierId == supplierId && o.OrderDate >= startDate && o.OrderDate <= endDate);

                return query.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving orders: {ex.Message}");
                throw new ApplicationException("Error retrieving orders", ex);
            }
        }
        public ImportOrder GetImportOrderBySupplierAndDate(int supplierId, DateTime date)
        {
            var query = _context.ImportOrders
                      .Include(o => o.Supplier)
                      .Include(o => o.ImportOrderItems)
                      .ThenInclude(oi => oi.Product)
                    .AsQueryable();

            return query.OrderByDescending(o=>o.UpdatedAt)
                .FirstOrDefault(o => o.SupplierId == supplierId && o.OrderDate == date);
        }
    }
}
