using COCOApp.Models;

namespace COCOApp.Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetCustomers();
        List<Customer> GetCustomers(int sellerId);
        List<Customer> GetCustomers(string nameQuery, int pageNumber, int pageSize, int sellerId, int statusId);
        Customer GetCustomerByIdForSeller(int customerId, int sellerId);
        Customer GetCustomerById(int customerId);
        int GetTotalCustomers(string nameQuery, int sellerId, int statusId);
        void AddCustomer(Customer customer);
        void EditCustomer(int customerId, Customer customer);
    }
}
