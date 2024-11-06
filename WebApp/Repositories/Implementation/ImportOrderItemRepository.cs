using COCOApp.Models;
using Microsoft.EntityFrameworkCore;

namespace COCOApp.Repositories.Implementation
{
    public class ImportOrderItemRepository : IImportOrderItemRepository
    {
        private readonly StoreManagerContext _context;

        public ImportOrderItemRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public void addImportOrderItem(ImportOrderItem item)
        {
            // Try to retrieve the existing order item if it exists
            var importOrderItem = _context.ImportOrderItems
                .Include(x => x.Order)
                .FirstOrDefault(o => o.OrderId == item.OrderId && o.ProductId == item.ProductId);

            if (importOrderItem == null)
            {
                // If the item does not exist, add it directly
                _context.ImportOrderItems.Add(item);
            }
            else
            {
                if (importOrderItem.Status)
                {
                    // Create a new ImportOrder if the status is true
                    var importOrder = new ImportOrder
                    {
                        SupplierId = importOrderItem.Order.SupplierId,
                        OrderDate = importOrderItem.Order.OrderDate,
                        Complete = false,
                        OrderTotal = 0,
                        SellerId = importOrderItem.Order.SellerId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    // Add the new import order
                    _context.ImportOrders.Add(importOrder);
                    _context.SaveChanges();  // Save to generate the new order ID

                    // Reassign the OrderId to the new import order
                    item.OrderId = importOrder.Id;

                    // Add the new import order item
                    _context.ImportOrderItems.Add(item);
                }
                else
                {
                    // If the status is false, just update the volume
                    importOrderItem.Volume += item.Volume;

                    // If you need the product, retrieve it with AsNoTracking
                    var product = _context.Products
                        .AsNoTracking()
                        .FirstOrDefault(p => p.Id == importOrderItem.ProductId);

                    // No further action required unless you need to modify product details
                }
            }

            _context.SaveChanges();  // Save changes at the end
        }

        public ImportOrderItem GetImportOrderItemById(int orderId, int productId, int sellerId)
        {
            var query = _context.ImportOrderItems
                                .Include(o => o.Product)
                                .Include(o => o.Order)
                                .ThenInclude(c => c.Supplier)
                                .AsQueryable();

            if (sellerId > 0)
            {
                query = query.Where(o => o.Order.SellerId == sellerId);
            }
            ImportOrderItem item = query.FirstOrDefault(u => u.OrderId == orderId && u.ProductId == productId);
            return item;
        }
        public List<ImportOrderItem> GetImportOrderItemsByIds(List<int> orderIds, int sellerId)
        {
            var query = _context.ImportOrderItems
                                  .Include(o => o.Product)
                                  .Include(o => o.Order)
                                  .ThenInclude(c => c.Supplier)
                                .Where(o => orderIds.Contains(o.OrderId))
                                .AsQueryable();

            if (sellerId > 0)
            {
                query = query.Where(o => o.Order.SellerId == sellerId);
            }

            return query.ToList();
        }
        public List<ImportOrderItem> GetImportOrderItems(int orderId, string nameQuery, int pageNumber, int pageSize, int sellerId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.ImportOrderItems.AsQueryable();
            query = query.Include(o => o.Product)
                         .Include(o => o.Order)
                         .ThenInclude(c => c.Supplier);
            if (sellerId > 0)
            {
                query = query.Where(o => o.Order.SellerId == sellerId);
            }
            if (orderId > 0)
            {
                query = query.Where(o => o.OrderId == orderId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(o => o.Order.Supplier.Name.Contains(nameQuery));
            }
            query = query.OrderByDescending(o => o.Order.OrderDate);

            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }
        public List<ImportOrderItem> GetImportOrderItems(int orderId, int sellerId)
        {

            var query = _context.ImportOrderItems.AsQueryable();
            query = query.Include(o => o.Product)
                         .Include(o => o.Order)
                         .ThenInclude(c => c.Supplier);
            if (sellerId > 0)
            {
                query = query.Where(o => o.Order.SellerId == sellerId);
            }
            if (orderId > 0)
            {
                query = query.Where(o => o.OrderId == orderId);
            }
            query = query.OrderByDescending(o => o.Order.OrderDate);

            return query.ToList();
        }

        public int GetTotalImportOrderItems(int orderId, string nameQuery, int sellerId)
        {
            var query = _context.ImportOrderItems.AsQueryable();
            query = query.Include(o => o.Product)
                         .Include(o => o.Order)
                         .ThenInclude(c => c.Supplier);
            if (sellerId > 0)
            {
                query = query.Where(o => o.Order.SellerId == sellerId);
            }
            if (orderId > 0)
            {
                query = query.Where(o => o.OrderId == orderId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Order.Supplier.Name.Contains(nameQuery));
            }

            return query.Count();
        }
        public void EditImportOrderItem(int orderId, int productId, ImportOrderItem order)
        {
            var existingOrder = _context.ImportOrderItems
                .Include(o => o.Product)
                .ThenInclude(i => i.InventoryManagement)
                .FirstOrDefault(c => c.OrderId == orderId && c.ProductId == productId);

            if (existingOrder != null)
            {
                existingOrder.OrderId = order.OrderId;
                existingOrder.ProductId = order.ProductId;
                existingOrder.ProductCost = order.ProductCost;
                existingOrder.Volume = order.Volume;
                existingOrder.RealVolume = order.RealVolume;
                //existingOrder.Total = order.Total;
                //existingOrder.Order.SellerId = order.Order.SellerId;
                existingOrder.CreatedAt = order.CreatedAt;
                existingOrder.UpdatedAt = order.UpdatedAt;
                existingOrder.Status = order.Status;
                if (existingOrder.Status)
                {
                    InventoryManagement inventory = _context.InventoryManagements.FirstOrDefault(p => p.ProductId == existingOrder.ProductId);
                    //Update inventory changes
                    int realVolumeAsInt = Convert.ToInt32(existingOrder.RealVolume);
                    inventory.RemainingVolume += realVolumeAsInt;
                }
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Order not found");
            }
        }
    }
}
