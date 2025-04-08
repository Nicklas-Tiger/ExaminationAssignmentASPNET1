using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class UserDismissedNotificationEntity
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public virtual UserEntity User { get; set; } = null!;


    [ForeignKey(nameof(Notification))]
    public string NotificationId { get; set; } = null!;
    public virtual NotificationEntity Notification { get; set; } = null!;
}