using Data.Contexts;
using Data.Entities;
using System;

namespace Data.Repositories;

public interface INotificationTypeRepository : IBaseRepository<NotificationTypeEntity, NotificationTypeEntity>
{
}

public class NotificationTypeRepository(DataContext context) : BaseRepository<NotificationTypeEntity, NotificationTypeEntity>(context), INotificationTypeRepository
{

}