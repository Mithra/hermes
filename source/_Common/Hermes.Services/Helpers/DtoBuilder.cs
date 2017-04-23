using System.Collections.Generic;
using System.Linq;
using Hermes.DataObjects.Application;
using Hermes.DataObjects.Channel;
using Hermes.DataObjects.Notification;
using Hermes.Entity.Models;

namespace Hermes.Services.Helpers
{
    public static class DtoBuilder
    {
        public static ApplicationDto ToDto(this Application application)
        {
            return new ApplicationDto {Id = application.application_id, Name = application.application_name};
        }

        public static ChannelDto ToDto(this Channel channel)
        {
            return new ChannelDto
            {
                Id = channel.channel_id,
                ApplicationId = channel.application_id,
                Name = channel.channel_name
            };
        }

        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto
            {
                ApplicationId = notification.Channel != null ? notification.Channel.application_id : 0,
                ApplicationName = (notification.Channel != null && notification.Channel.Application != null) ? notification.Channel.Application.application_name : null,

                ChannelId = notification.channel_id,
                ChannelName = notification.Channel != null ? notification.Channel.channel_name : "",

                Id = notification.notification_id,
                Persistent = true,
                Time = notification.notification_time,
                Message = notification.notification_message,
                Code = notification.notification_code,
                Tags = notification.NotificationTags != null ? notification.NotificationTags.ToDictionary(t => t.notification_tag_key, t => t.notification_tag_value) : new Dictionary<string, string>()
            };
        }
    }
}