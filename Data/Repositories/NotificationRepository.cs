using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Domain.Extensions;

namespace Data.Repositories;

public interface INotificationRepository : IBaseRepository<NotificationEntity, NotificationEntity>
{
    Task<NotificationResult<Notification>> GetLatestNotification();
}


public class NotificationRepository(DataContext context) : BaseRepository<NotificationEntity, NotificationEntity>(context), INotificationRepository
{
    public async Task<NotificationResult<Notification>> GetLatestNotification()
    {
        var entity = await _table.OrderByDescending(x => x.CreateDate).FirstOrDefaultAsync();
        var notification = entity!.MapTo<Notification>();
        return new NotificationResult<Notification> { Succeeded = true, StatusCode = 200, Result = notification };
    }
}