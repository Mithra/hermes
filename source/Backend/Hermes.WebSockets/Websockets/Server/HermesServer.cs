using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hermes.DataObjects.Notification;
using Hermes.Entity;
using Hermes.Entity.Models;
using Hermes.Services;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Collections;
using Hermes.Services.Helpers.Logging;
using NLog;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Hermes.WebSockets.Websockets.Server
{
    public class HermesServer : SuperSocket.WebSocket.WebSocketServer<HermesSession>
    {
        private readonly AppLogger _log = GlobalAppLogger.GetLogger(LogManager.GetCurrentClassLogger());

        private readonly Object _lockObj;
        private readonly Dictionary<int, Dictionary<string, HashSet<string>>> _channelSubscriptions;

        private readonly ManualResetEvent _threadEvent;
        private Task _rmqTask;
        private readonly CachedDictionary<string, Application> _cachedApplications;
        
        public HermesServer()
        {
            _lockObj = new object();
            _threadEvent = new ManualResetEvent(false);
            _channelSubscriptions = new Dictionary<int, Dictionary<string, HashSet<string>>>();

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

        public override bool Start()
        {
            if (!base.Start())
                return false;

            _rmqTask = Task.Factory.StartNew(() => StartRmqListener(_threadEvent));
            return true;
        }

        public override void Stop()
        {
            _threadEvent.Set();
            _rmqTask.Wait();

            base.Stop();
        }

        public void RegisterClient(string sessionId, string applicationName, List<string> channels)
        {
            Logger.DebugFormat("Register {0}", sessionId);

            Application application = _cachedApplications.GetByKey(applicationName);
            if (application == null)
                throw new Exception("Application not found: " + applicationName);

            if (channels == null || channels.Count == 0)
                throw new Exception("At least one channel name must be specified");

            int applicationId = application.application_id;

            lock (_lockObj)
            {
                // Clear previous subscriptions
                ClearClientSubscriptions(sessionId);

                if (!_channelSubscriptions.ContainsKey(applicationId))
                    _channelSubscriptions[applicationId] = new Dictionary<string, HashSet<string>>();

                Dictionary<string, HashSet<string>> applicationChannelRegistrations = _channelSubscriptions[applicationId];

                // Add session to all desired channels
                foreach (string channelName in channels)
                {
                    Logger.DebugFormat(" -> {0}", channelName);

                    if (!applicationChannelRegistrations.ContainsKey(channelName))
                        applicationChannelRegistrations[channelName] = new HashSet<string>();

                    applicationChannelRegistrations[channelName].Add(sessionId);
                }
            }
        }

        public void UnregisterClient(string sessionId)
        {
            lock (_lockObj)
            {
                Logger.DebugFormat("Unregister {0}", sessionId);
                ClearClientSubscriptions(sessionId);
            }
        }

        private void ClearClientSubscriptions(string sessionId)
        {
            // Remove session from all channels subscriptions
            foreach (KeyValuePair<int, Dictionary<string, HashSet<string>>> applicationSubscriptions in _channelSubscriptions)
            {
                foreach (KeyValuePair<string, HashSet<string>> channelSubscriptions in applicationSubscriptions.Value)
                {
                    if (channelSubscriptions.Value.Contains(sessionId))
                    {
                        Logger.DebugFormat(" -> {0}", channelSubscriptions.Key);
                        channelSubscriptions.Value.Remove(sessionId);
                    }
                }
            }
        }

        public void SendLastNotifications(HermesSession session, Guid clientId, string applicationName, List<string> channels, int nbNotification, long? lastNotificationId)
        {
            NotificationService service = new NotificationService();

            Application application = _cachedApplications.GetByKey(applicationName);
            if (application == null)
                return;

            List<long> channelIds;
            using (HermesContext db = new HermesContext())
                channelIds = db.Channels.Where(c => c.application_id == application.application_id && channels.Contains(c.channel_name)).Select(c => c.channel_id).ToList();

            List<NotificationDto> lastNotifications = service.GetLastNotifications(_log, clientId, channelIds, nbNotification, lastNotificationId);

            foreach (NotificationDto lastNotification in lastNotifications)
                session.Send(EAnswerType.NewNotification, lastNotification);
        }

        public void OnNotificationCreated(NotificationDto notification)
        {
            int applicationId = notification.ApplicationId;
            string channelName = notification.ChannelName;

            lock (_lockObj)
            {
                if (!_channelSubscriptions.ContainsKey(applicationId))
                    return;

                Dictionary<string, HashSet<string>> subscriptions = _channelSubscriptions[applicationId];
                if (!subscriptions.ContainsKey(channelName))
                    return;

                HashSet<string> sessionIds = subscriptions[channelName];
                foreach (HermesSession session in GetSessions(s => sessionIds.Contains(s.SessionID)))
                    session.Send(EAnswerType.NewNotification, notification);
            }
        }

        private void StartRmqListener(ManualResetEvent eventTrigger)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = Settings.RmqHost,
                Port = Settings.RmqPort,
                UserName = Settings.RmqUsername,
                Password = Settings.RmqPassword,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: Settings.RmqExchangeName, type: "fanout");

                string queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName, exchange: Settings.RmqExchangeName, routingKey: "");

                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    OnNotificationCreated(ProtoBufHelper.Deserialize<NotificationDto>(ea.Body));
                };
                consumer.Shutdown += (model, args) =>
                {
                    _log.IndentError(args.ReplyText);
                };
                channel.BasicConsume(queue: queueName, noAck: true, consumer: consumer);
                eventTrigger.WaitOne();
            }
        }
    }
}