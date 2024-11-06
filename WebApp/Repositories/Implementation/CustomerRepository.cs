using COCOApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace COCOApp.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly StoreManagerContext _context;

        public CustomerRepository(StoreManagerContext context)
        {
            _context = context;
        }

        public List<Customer> GetCustomers()
        {
            return _context.Customers.AsQueryable().ToList();
        }
        public List<Customer> GetCustomers(int sellerId)
        {
            var query = _context.Customers.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            return query.ToList();
        }

        public List<Customer> GetCustomers(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId)
        {
            pageNumber = Math.Max(pageNumber, 1);

            var query = _context.Customers.AsQueryable();
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

        public Customer GetCustomerByIdForSeller(int customerId, int sellerId)
        {
            var query = _context.Customers.AsQueryable();
            if (sellerId > 0)
            {
                query = query.Where(c => c.SellerId == sellerId);
            }
            return customerId > 0 ? query.FirstOrDefault(u => u.Id == customerId) : null;
        }
        public Customer GetCustomerById(int customerId)
        {
            var query = _context.Customers.AsQueryable();
            return customerId > 0 ? query.FirstOrDefault(u => u.Id == customerId) : null;
        }

        public int GetTotalCustomers(string nameQuery, int sellerId, int statusId)
        {
            var query = _context.Customers.AsQueryable();
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

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void EditCustomer(int customerId, Customer customer)
        {
            var existingCustomer = _context.Customers.FirstOrDefault(c => c.Id == customerId);

            if (existingCustomer != null)
            {
                existingCustomer.Name = customer.Name;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;
                existingCustomer.Status = customer.Status;
                existingCustomer.Note = customer.Note;
                existingCustomer.UpdatedAt = customer.UpdatedAt;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Customer not found");
            }
        }
    }
}
