using Hermes.DataObjects.Notification;

namespace Hermes.Services
{
    public interface INotificationListener
    {
        void EnqueueNotification(NotificationDto dto);
    }
}
