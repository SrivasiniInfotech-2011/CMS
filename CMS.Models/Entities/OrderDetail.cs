namespace CMS.Models.Entities
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public string ItemName { get; set; }
        public string ItemSku { get; set; }
        public string Manufacturer { get; set; }
        public virtual Order Order { get; set; }
    }
}
