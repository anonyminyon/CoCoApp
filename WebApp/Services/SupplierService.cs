using COCOApp.Models;
using COCOApp.Repositories;
using System.Collections.Generic;

namespace COCOApp.Services
{
    public class SupplierService : StoreManagerService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public List<Supplier> GetSuppliers()
        {
            return _supplierRepository.GetSuppliers();
        }

        public List<Supplier> GetSuppliers(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            return _supplierRepository.GetSuppliers(nameQuery, pageNumber, pageSize, sellerId, statusId);
        }

        public Supplier GetSupplierById(int supplierId, int sellerId)
        {
            return _supplierRepository.GetSupplierById(supplierId, sellerId);
        }
        public Supplier GetSupplierById(int supplierId)
        {
            return _supplierRepository.GetSupplierById(supplierId);
        }
        public int GetTotalSuppliers(string nameQuery, int sellerId, int statusId)
        {
            return _supplierRepository.GetTotalSuppliers(nameQuery, sellerId, statusId);
        }

        public void AddSupplier(Supplier supplier)
        {
            _supplierRepository.AddSupplier(supplier);
        }

        public void EditSupplier(int supplierId, Supplier supplier)
        {
            _supplierRepository.EditSupplier(supplierId, supplier);
        }

        public List<Supplier> GetAllMySuppliers(int supplierId)
        {
            return _supplierRepository.GetAllMySupplier(supplierId);
        }
    }
}