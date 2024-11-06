using COCOApp.Models;
using COCOApp.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace COCOApp.Services
{
    public class InventoryMangementService: StoreManagerService
    {
        private readonly IInventoryManagementRepository _inventoryManagementRepository;
        public InventoryMangementService(IInventoryManagementRepository inventoryManagementRepository)
        {
            _inventoryManagementRepository = inventoryManagementRepository;
        }
        public void AddInventory(InventoryManagement inventory)
        {
            _inventoryManagementRepository.AddInventory(inventory);
        }
    }
}
