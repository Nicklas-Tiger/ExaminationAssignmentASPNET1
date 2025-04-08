using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class NotificationTypeEntity
{
    [Key]
    public int Id { get; set; }
    public string TypeName { get; set; } = null!;

    public virtual ICollection<NotificationEntity> Notifications { get; set; } = [];
}
