using Business.Models;
using Data.Contexts;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Data.Repositories;

public interface IUserDismissedNotificationRepository : IBaseRepository<UserDismissedNotificationEntity, UserDismissedNotificationEntity>
{
    Task<RepositoryResult<IEnumerable<string>>> GetNotificationsIdsAsync(string userId);
}

public class UserDismissedNotificationRepository(DataContext context) : BaseRepository<UserDismissedNotificationEntity, UserDismissedNotificationEntity>(context), IUserDismissedNotificationRepository
{
    public async Task<RepositoryResult<IEnumerable<string>>> GetNotificationsIdsAsync(string userId)
    {
        var ids = await _table.Where(x => x.UserId == userId).Select(x => x.NotificationId).ToListAsync();
        return new RepositoryResult<IEnumerable<string>> { Succeeded = true, StatusCode = 200, Result = ids };
    }
}
