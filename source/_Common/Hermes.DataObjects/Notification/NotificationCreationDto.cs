using System.Collections.Generic;

namespace Hermes.DataObjects.Notification
{
    public class NotificationCreationDto
    {
        public string ApplicationName { get; set; }
        public string ChannelName { get; set; }

        public string Code { get; set; }
        public string Message { get; set; }
        public bool Persistent { get; set; }

        public Dictionary<string, string> Tags { get; set; } 
    }
}
