using CMS.Models.Domain;
using CMS.Models.Entities;
using CMS.Repository.Repositories;

namespace CMS.Services.Customers
{
    public class CustomerService(IRepository<Customer> repository) : ICustomerService
    {
        public CustomerModel AddCustomer(Customer customer)
        {
            customer=repository.Add(customer);
            return new CustomerModel
            {
                Id= customer.Id,
                Dob = customer.Dob,
                FirstName=customer.FirstName,
                LastName=customer.LastName,
                MiddleName = customer.MiddleName,
            };
        }

        public IEnumerable<CustomerModel> GetAllCustomers()
        {
            var customers = repository.GetAll();
            return customers.Select(customer => new CustomerModel
            {
                Id = customer.Id,
                Dob = customer.Dob,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                MiddleName = customer.MiddleName,
            });
        }

        public CustomerModel? SearchCustomer(Func<Customer, bool> customerSearchQuery)
        {
            var customers = repository.GetBySearchParams(customerSearchQuery);
            var customer = customers.FirstOrDefault();
            return customer == null ? null : new CustomerModel
            {
                Id = customer.Id,
                Dob = customer.Dob,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                MiddleName = customer.MiddleName,
            };
        }

        public CustomerModel UpdateCustomer(Customer customer)
        {
            customer = repository.Update(customer);
            return new CustomerModel
            {
                Id = customer.Id,
                Dob = customer.Dob,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                MiddleName = customer.MiddleName,
            };
        }
    }
}
