using CMS.Models.Domain;
using CMS.Models.Entities;
using CMS.Repository.Repositories;

namespace CMS.Services.CustomerOrders
{
    public class CustomerOrderService(IRepository<Order> repository) : ICustomerOrderService
    {
        public CustomerOrderViewModel AddCustomerOrder(Order customerOrder)
        {
            var order = repository.Add(customerOrder);
            return new CustomerOrderViewModel
            {
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                CustoerOrders = order.OrderDetails.Select(od => new CustomerOrder
                {
                    OrderDate = order.OrderDate,
                    ItemName=od.ItemName
                   
                }).ToList()
            };
        }

        public IEnumerable<CustomerOrderViewModel> GetAllCustomerOrders()
        {
            var orders = repository.GetAll();
            return orders.Select(order => new CustomerOrderViewModel
            {
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                CustoerOrders = order.OrderDetails.Select(od => new CustomerOrder
                {
                    OrderDate = order.OrderDate,
                    ItemName = od.ItemName
                }).ToList()
            });
        }

        public CustomerOrderViewModel? SearchCustomerOrder(Func<CustomerOrder, bool> customerOrderSearchQuery)
        {
            var orders = repository.GetBySearchParams(o => o.OrderDetails.Any(od => customerOrderSearchQuery(new CustomerOrder
            {
                OrderDate = o.OrderDate,
                ItemName = od.ItemName
            })));
            return orders.Select(order => new CustomerOrderViewModel
            {
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                CustoerOrders = order.OrderDetails.Select(od => new CustomerOrder
                {
                    OrderDate = order.OrderDate,
                    ItemName = od.ItemName
                }).ToList()
            }).FirstOrDefault();
        }

        public CustomerOrderViewModel UpdateCustomerOrder(Order customerOrder)
        {
            var order = repository.Update(customerOrder);
            return new CustomerOrderViewModel
            {
                FirstName = order.Customer.FirstName,
                LastName = order.Customer.LastName,
                CustoerOrders = order.OrderDetails.Select(od => new CustomerOrder
                {
                    OrderDate = order.OrderDate,
                    ItemName = od.ItemName
                }).ToList()
            };
        }
    }
}
