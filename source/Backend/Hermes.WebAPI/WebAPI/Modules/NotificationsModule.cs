using System;
using System.Collections.Generic;
using Hermes.DataObjects.Notification;
using Hermes.Services;
using Nancy.TinyIoc;

namespace Hermes.WebAPI.WebAPI.Modules
{
    public class NotificationsModule : CommonModule
    {
        private readonly NotificationService _service;

        public NotificationsModule()
            : base("/notification")
        {
            _service = new NotificationService(TinyIoCContainer.Current.Resolve<INotificationListener>());

            // Get all notifications
            Get[""] = x => CallWithResponse("get-all-notifications", logger =>
            {
                int pageIndex = (int) Request.Query.pageIndex;
                int maxItems = (int) Request.Query.maxItems;
                List<long> channelIds = GetList<long>(Request.Query.channelIds);

                return _service.GetNotifications(logger, channelIds, pageIndex, maxItems);
            });

            // Get latest notifications
            Get["latest"] = x => CallWithResponse("get-latest-notifications", logger =>
            {
                Guid clientId = (Guid)Request.Query.clientId;
                int maxNotifications = (int)Request.Query.maxNotifications;
                long lastNotificationId = (long)Request.Query.lastNotificationId;
                List<long> channelIds = GetList<long>(Request.Query.channelIds);

                return _service.GetLastNotifications(logger, clientId, channelIds, maxNotifications, lastNotificationId);
            });

            // Push new notification
            Post[""] = x => PostRequest<NotificationCreationDto>("create-notification", (logger, input) => _service.CreateNotification(logger, input));

            // Aknowledge notification
            Put["{notificationId:long}/aknowledge"] = x => CallWithStatus("aknowledge-notification", logger => _service.AknowledgeNotification(logger, x.notificationId, Request.Query.clientId));
        }
    }
}
