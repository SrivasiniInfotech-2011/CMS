using CMS.Models.Domain;
using CMS.Models.Entities;
using CMS.Repository.Repositories;

namespace CMS.Services.Customers
{
    public class CustomerService(IRepository<Customer> repository) : ICustomerService
    {
        public CustomerModel AddCustomer(Customer customer)
        {
            customer = repository.Add(customer);
            return ToModel(customer);
        }

        public IEnumerable<CustomerModel> GetAllCustomers()
        {
            return repository.GetAll().Select(ToModel);
        }

        public Customer? SearchCustomer(Func<Customer, bool> customerSearchQuery)
        {
            var customers = repository.GetBySearchParams(customerSearchQuery);
            return customers.FirstOrDefault();
        }

        public CustomerModel UpdateCustomer(Customer customer)
        {
            customer = repository.Update(customer);
            return ToModel(customer);
        }

        private static CustomerModel ToModel(Customer customer) => new()
        {
            Id = customer.Id,
            Dob = customer.Dob,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MiddleName = customer.MiddleName,
            ImageUrl = customer.ImageBlobName == null ? null : $"/api/Customer/{customer.Id}/image"
        };
    }
}
