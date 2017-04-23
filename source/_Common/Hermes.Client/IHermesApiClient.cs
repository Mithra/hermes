using System;
using System.Collections.Generic;
using Hermes.DataObjects.Application;
using Hermes.DataObjects.Channel;
using Hermes.DataObjects.Notification;

namespace Hermes.Client
{
    public interface IHermesApiClient
    {
        // Applications
        ApplicationDto GetApplication(int applicationId);
        List<ApplicationDto> GetApplications();

        // Channels
        ChannelDto GetChannel(int channelId);
        List<ChannelDto> GetChannels();
        List<ChannelDto> GetApplicationChannels(int applicationId);
        long CreateChannel(ChannelCreationDto channel);
        //void DeleteChannel(int channelId);

        // Notifications
        long? PushNotification(NotificationCreationDto notification);
        void AknowledgeNotification(long notificationId, Guid clientId);
    }
}
