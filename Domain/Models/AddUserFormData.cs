using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class AddUserFormData
{
    public IFormFile? Image { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Address { get; set; }
    //public DateTime? BirthDate { get; set; } Implementera sen!!!

}
