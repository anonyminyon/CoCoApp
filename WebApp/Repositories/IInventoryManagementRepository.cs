using COCOApp.Models;

namespace COCOApp.Repositories
{
    public interface IInventoryManagementRepository
    {
        void AddInventory(InventoryManagement inventoryManagement);
        void EditInventory(int productId, InventoryManagement inventoryManagement);
    }
}
