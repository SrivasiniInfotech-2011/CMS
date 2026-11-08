using CMS.Models.Domain;
using CMS.Models.Entities;

namespace CMS.Services.Customers
{
    public interface ICustomerService
    {
        IEnumerable<CustomerModel> GetAllCustomers();
        Customer? SearchCustomer(Func<Customer, bool> customerSearchQuery);
        CustomerModel AddCustomer(Customer customer);
        CustomerModel UpdateCustomer(Customer customer);
    }
}
