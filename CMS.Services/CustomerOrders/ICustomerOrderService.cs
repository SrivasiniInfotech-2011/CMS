using CMS.Models.Domain;
using CMS.Models.Entities;

namespace CMS.Services.CustomerOrders
{
    public interface ICustomerOrderService
    {
        IEnumerable<CustomerOrderViewModel> GetAllCustomerOrders();
        CustomerOrderViewModel? SearchCustomerOrder(Func<CustomerOrder, bool> customerOrderSearchQuery);
        CustomerOrderViewModel AddCustomerOrder(Order customerOrder);
        CustomerOrderViewModel UpdateCustomerOrder(Order customerOrder);
    }
}
