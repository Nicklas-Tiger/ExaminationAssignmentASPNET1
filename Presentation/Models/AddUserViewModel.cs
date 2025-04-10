using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class AddUserViewModel
{
    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "First Name", Prompt = "Enter first name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name", Prompt = "Enter last name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;
    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone", Prompt = "Enter phone number (optinal)")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Job Title", Prompt = "Enter job title (optinal)")]
    public string? JobTitle { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Address", Prompt = "Enter address")]
    public string? Address { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime BirthDate { get; set; }

    public IEnumerable<UsersViewModel> TeamMembers { get; set; } = null!;


}