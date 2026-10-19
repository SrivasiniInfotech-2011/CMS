namespace CMS.Models.Domain;

public class CustomerModel
{
    public int? Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string MiddleName { get; set; }
    public DateTime? Dob { get; set; }
}
