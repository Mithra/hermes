using System;
using System.Collections.Generic;

namespace Hermes.DataObjects.Notification
{
    public class NotificationDto
    {
        public long? Id { get; set; }
        public bool Persistent { get; set; }

        public long ChannelId { get; set; }
        public string ChannelName { get; set; }

        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }

        public DateTime Time { get; set; }

        public string Code { get; set; }
        public string Message { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Persistent: {1}, ChannelId: {2}, ChannelName: {3}, ApplicationId: {4}, ApplicationName: {5}, Time: {6}, Code: {7}, Message: {8}", Id, Persistent, ChannelId, ChannelName, ApplicationId, ApplicationName, Time, Code, Message);
        }
    }
}