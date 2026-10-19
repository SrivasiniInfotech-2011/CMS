namespace CMS.Models.Entities;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime? Dob { get; set; }
    public virtual IEnumerable<Order> Orders { get; set; }

}
