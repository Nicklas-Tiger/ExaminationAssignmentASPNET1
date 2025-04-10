using System.ComponentModel.DataAnnotations;

namespace Presentation.Models;

public class EditUserViewModel
{
    [Required]
    public string Id { get; set; } = null!;
    [DataType(DataType.Upload)]
    [Display(Name = "Project Image", Prompt = "Select project image")]
    public IFormFile? Image { get; set; }
    public string? ImageUrl { get; set; }
    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "First Name", Prompt = "Enter first name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name", Prompt = "Enter last name")]
    public string LastName { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Job Title", Prompt = "Enter job title (optinal)")]
    public string? JobTitle { get; set; }

    [DataType(DataType.PhoneNumber)]
    [Display(Name = "Phone", Prompt = "Enter phone number (optinal)")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email", Prompt = "Enter email address")]
    public string Email { get; set; } = null!;

    [DataType(DataType.Text)]
    [Display(Name = "Street name", Prompt = "Enter street name")]
    public string? Streetname { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "Postal code", Prompt = "Enter postal code")]
    public string? PostalCode { get; set; }

    [DataType(DataType.Text)]
    [Display(Name = "City", Prompt = "Enter city")]
    public string? City { get; set; }
}
