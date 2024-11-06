using COCOApp.Models;
using Microsoft.EntityFrameworkCore;

namespace COCOApp.Repositories.Implementation
{
    public class InventoryManagementRepository : IInventoryManagementRepository
    {
        private readonly StoreManagerContext _context;

        public InventoryManagementRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public void AddInventory(InventoryManagement inventoryManagement)
        {
            _context.InventoryManagements.Add(inventoryManagement);
            _context.SaveChanges();
        }

        public void EditInventory(int productId, InventoryManagement inventoryManagement)
        {
            throw new NotImplementedException();
        }
    }
}
