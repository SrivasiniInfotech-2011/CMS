using System.ComponentModel.DataAnnotations;

namespace CMS.Models.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderValue { get; set; }
        public int OrderStatus { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
