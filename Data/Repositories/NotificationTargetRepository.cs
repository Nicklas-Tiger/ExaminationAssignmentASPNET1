using Data.Contexts;
using Data.Entities;
using System;

namespace Data.Repositories;

public interface INotificationTargetRepository : IBaseRepository<NotificationTargetEntity, NotificationTargetEntity>
{
}


public class NotificationTargetRepository(DataContext context) : BaseRepository<NotificationTargetEntity, NotificationTargetEntity>(context), INotificationTargetRepository
{

}