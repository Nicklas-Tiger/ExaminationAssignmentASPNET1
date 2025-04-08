using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    [ProtectedPersonalData]
    public string? Image { get; set; }

    [ProtectedPersonalData]
    public string? FirstName { get; set; }

    [ProtectedPersonalData]
    public string? LastName { get; set; }

    [ProtectedPersonalData]
    public string? JobTitle { get; set; }

    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];

    public virtual ICollection<UserDismissedNotificationEntity> DismissedNotifications { get; set; } = [];

}
