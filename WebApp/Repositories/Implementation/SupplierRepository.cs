using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace COCOApp.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly StoreManagerContext _context;

        public SupplierRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<Supplier> GetSuppliers()
        {
            return _context.Suppliers.AsQueryable().ToList();
        }

        public List<Supplier> GetSuppliers(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.Suppliers.AsQueryable();
            bool status = statusId == 1;
            if (statusId > 0)
            {
                query = query.Where(p => p.Status == status);
            }
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Name.Contains(nameQuery));
            }
            query = query.OrderByDescending(c => c.Id);
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public Supplier GetSupplierById(int supplierId, int sellerId)
        {
            var query = _context.Suppliers.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            return supplierId > 0 ? query.FirstOrDefault(u => u.Id == supplierId) : null;
        }

        public int GetTotalSuppliers(string nameQuery, int sellerId, int statusId)
        {
            var query = _context.Suppliers.AsQueryable();
            bool status = statusId == 1;
            if (statusId > 0)
            {
                query = query.Where(p => p.Status == status);
            }
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            if (!string.IsNullOrEmpty(nameQuery))
            {
                query = query.Where(c => c.Name.Contains(nameQuery));
            }

            return query.Count();
        }

        public void AddSupplier(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
        }

        public void EditSupplier(int supplierId, Supplier supplier)
        {
            var existingSupplier = _context.Suppliers.FirstOrDefault(c => c.Id == supplierId);

            if (existingSupplier != null)
            {
                existingSupplier.Name = supplier.Name;
                existingSupplier.Phone = supplier.Phone;
                existingSupplier.Address = supplier.Address;
                existingSupplier.Status = supplier.Status;
                existingSupplier.Note = supplier.Note;
                existingSupplier.UpdatedAt = supplier.UpdatedAt;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Supplier not found");
            }
        }

        public Supplier GetSupplierById(int supplierId)
        {
            var query = _context.Suppliers.AsQueryable();
            return supplierId > 0 ? query.FirstOrDefault(u => u.Id == supplierId) : null;
        }

        public List<Supplier> GetAllMySupplier(int supplierId)
        {
            return _context.Suppliers.Where(s => s.SellerId == supplierId).AsQueryable().ToList();
        }
    }
}
