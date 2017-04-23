using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hermes.DataObjects.Notification;
using Hermes.Entity;
using Hermes.Entity.Models;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Collections;
using Hermes.Services.Helpers.Logging;

namespace Hermes.Services
{
    public class NotificationService
    {
        private static CachedDictionary<string, Application> _cachedApplications;
        private static CachedDictionary<Tuple<int, string>, Channel> _cachedChannels;

        private readonly INotificationListener _listener;

        public NotificationService(INotificationListener listener = null)
        {
            _listener = listener;

            if (_cachedApplications == null)
            {
                _cachedApplications = new CachedDictionary<string, Application>
                {
                    CacheFullResetDelay = TimeSpan.FromHours(1),
                    GetAllCallback = () =>
                    {
                        using (HermesContext db = new HermesContext())
                            return db.Applications.ToDictionary(a => a.application_name, a => a);
                    }
                };
            }

            if (_cachedChannels == null)
            {
                _cachedChannels = new CachedDictionary<Tuple<int, string>, Channel>
                {
                    CacheEntryRefreshDelay = TimeSpan.FromHours(1),
                    GetByKeyCallback = key =>
                    {
                        using (HermesContext db = new HermesContext())
                            return db.Channels.FirstOrDefault(c => c.application_id == key.Item1 && c.channel_name == key.Item2);
                    }
                };
            }
        }

        public NotificationCreationResultDto CreateNotification(AppLogger logger, NotificationCreationDto dto)
        {
            logger.IndentInfo("CreateNotification [applicationName={0}; channelName='{1}'; message='{2}']", dto.ApplicationName, dto.ChannelName, dto.Message);

            using (HermesContext db = new HermesContext())
            {
                Application application = _cachedApplications.GetByKey(dto.ApplicationName);
                if (application == null)
                    throw new Exception("Application not found: " + dto.ApplicationName);

                Channel channel = _cachedChannels.GetByKey(new Tuple<int, string>(application.application_id, dto.ChannelName));

                // If the channel does not exist already, create it
                if (channel == null)
                {
                    logger.IndentInfo("Channel not found, creating one");
                    channel = db.Channels.Add(new Channel
                    {
                        application_id = application.application_id,
                        channel_name = dto.ChannelName
                    });
                    db.SaveChanges();

                    _cachedChannels.Add(new Tuple<int, string>(application.application_id, dto.ChannelName), channel);
                }

                // Create notification
                long? notificationId = null;

                return db.ExecuteInTransaction(ctx =>
                {
                    if (dto.Persistent)
                    {
                        Notification notif = ctx.Notifications.Add(new Notification
                        {
                            channel_id = channel.channel_id,
                            notification_time = DateTime.UtcNow,
                            notification_code = dto.Code,
                            notification_message = dto.Message
                        });
                        ctx.SaveChanges();
                        notificationId = notif.notification_id;

                        if (dto.Tags != null)
                        {
                            foreach (KeyValuePair<string, string> tag in dto.Tags)
                                ctx.NotificationTags.Add(new NotificationTag
                                {
                                    notification_id = notif.notification_id,
                                    notification_tag_key = tag.Key,
                                    notification_tag_value = tag.Value
                                });

                            ctx.SaveChanges();
                        }
                    }

                    // Send to RMQ
                    if (_listener != null)
                    {
                        _listener.EnqueueNotification(new NotificationDto
                        {
                            Id = notificationId,
                            Persistent = dto.Persistent,

                            ApplicationId = application.application_id,
                            ApplicationName = application.application_name,

                            ChannelId = channel.channel_id,
                            ChannelName = channel.channel_name,

                            Code = dto.Code,
                            Message = dto.Message,
                            Time = DateTime.UtcNow,

                            Tags = dto.Tags ?? new Dictionary<string, string>()
                        });
                    }

                    return new NotificationCreationResultDto { NotificationId = notificationId };
                });
            }
        }

        public void AknowledgeNotification(AppLogger logger, long notificationId, Guid clientId)
        {
            using (HermesContext db = new HermesContext())
            {
                bool notificationExists = db.Notifications.Any(n => n.notification_id == notificationId);
                if (!notificationExists)
                    throw new Exception("Notification not found: " + notificationId);

                bool alreadyAknowledged = db.NotificationClientReceipts.Any(r => r.notification_id == notificationId && r.client_id == clientId);
                if (alreadyAknowledged)
                    return; // Don't do anything, the notification has already been aknowledged

                db.NotificationClientReceipts.Add(new NotificationClientReceipt
                {
                    notification_id = notificationId,
                    client_id = clientId
                });
                db.SaveChanges();
            }
        }

        public List<NotificationDto> GetLastNotifications(AppLogger logger, Guid clientId, List<long> channels, int maxNotifications, long? lastNotificationId)
        {
            using (HermesContext db = new HermesContext())
            {
                // TODO: Use a raw SQL query to improve performance
                IQueryable<Notification> query = db.Notifications
                    .Where(n => channels.Contains(n.channel_id)
                                && !db.NotificationClientReceipts.Any(c => c.notification_id == n.notification_id && c.client_id == clientId));

                if (lastNotificationId.HasValue && lastNotificationId > 0)
                    query = query.Where(n => n.notification_id > lastNotificationId.Value);

                List<Notification> notifications = query
                    .Include(n => n.Channel.Application)
                    .Include(n => n.NotificationTags)
                    .OrderByDescending(n => n.notification_time)
                    .Take(maxNotifications)
                    .OrderBy(n => n.notification_time)
                    .ToList();

                return notifications.Select(n => n.ToDto()).ToList();
            }
        }

        public PaginatedResults<NotificationDto> GetNotifications(AppLogger logger, List<long> channelIds, int pageIndex, int maxNotifications)
        {
            using (HermesContext db = new HermesContext())
            {
                IQueryable<Notification> query = db.Notifications
                    .Include(n => n.Channel.Application)
                    .Include(n => n.NotificationTags)
                    .Where(n => channelIds.Contains(n.channel_id));

                int totalItems = query.Count();

                // Paginate
                List<Notification> notifications = query
                    .OrderByDescending(n => n.notification_time)
                    .Skip(pageIndex*maxNotifications)
                    .Take(maxNotifications)
                    .ToList();

                return new PaginatedResults<NotificationDto>
                {
                    PageIndex = pageIndex,
                    TotalNumber = totalItems,
                    Results = notifications.Select(n => n.ToDto()).ToList()
                };
            }
        }
    }
}