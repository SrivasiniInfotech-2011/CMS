using System.ComponentModel.DataAnnotations;

namespace CMS.Models.Requests;

public sealed class CreateCustomerRequest
{
    [Required, StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(100)]
    public string MiddleName { get; set; } = string.Empty;

    public DateTime? Dob { get; set; }
}
