using System;
using System.Collections.Generic;
using Hermes.WebSockets.Websockets.Server;
using Newtonsoft.Json;
using SuperSocket.WebSocket.SubProtocol;

namespace Hermes.WebSockets.Websockets.Commands
{
    public class RegisterCmd : SubCommandBase<HermesSession>
    {
        public class RegisterCmdParameters
        {
            public string ApplicationName { get; set; }
            public Guid ClientId { get; set; }
            public List<string> Channels { get; set; }

            public int? FetchLast { get; set; }
            public long? LastNotificationId { get; set; }
        }

        public override string Name
        {
            get { return "register"; }
        }

        public override void ExecuteCommand(HermesSession session, SubRequestInfo requestInfo)
        {
            try
            {
                RegisterCmdParameters parameters = JsonConvert.DeserializeObject<RegisterCmdParameters>(requestInfo.Body);
                session.HermesServer.RegisterClient(session.SessionID, parameters.ApplicationName, parameters.Channels);
                
                // Send last X unread notifications
                if (parameters.FetchLast.HasValue && parameters.FetchLast.Value > 0)
                    session.HermesServer.SendLastNotifications(session, parameters.ClientId, parameters.ApplicationName, parameters.Channels, parameters.FetchLast.Value, parameters.LastNotificationId);
            }
            catch (Exception e)
            {
                session.Send("Unable to register: " + e.Message);
            }
        }
    }
}
