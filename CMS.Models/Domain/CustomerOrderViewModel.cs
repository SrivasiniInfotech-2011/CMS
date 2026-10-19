using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Models.Domain
{
    public class CustomerOrderViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<CustomerOrder> CustoerOrders { get; set; }
    }
}
