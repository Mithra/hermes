using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Hermes.DataObjects.Notification;
using Hermes.Services;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Logging;
using NLog;
using RabbitMQ.Client;

namespace Hermes.WebAPI.Rmq
{
    public class RmqNotifier : INotificationListener
    {
        private readonly AppLogger _log = GlobalAppLogger.GetLogger(LogManager.GetCurrentClassLogger());

        private readonly ConcurrentQueue<NotificationDto> _pendingNotifications;
        private readonly ManualResetEvent _threadEvent;
        private bool _ack;
        private bool _stopRequested;

        public RmqNotifier()
        {
            _threadEvent = new ManualResetEvent(false);
            _pendingNotifications = new ConcurrentQueue<NotificationDto>();
            _stopRequested = false;
            _ack = false;
        }

        public void Start()
        {
            Task.Factory.StartNew(NotificationLoop);
        }

        public void Stop()
        {
            _stopRequested = true;
            _threadEvent.WaitOne();
        }

        public void EnqueueNotification(NotificationDto dto)
        {
            _pendingNotifications.Enqueue(dto);
        }

        private void NotificationLoop()
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

                channel.BasicAcks += (sender, args) =>
                {
                    _ack = true;
                };

                channel.BasicNacks += (sender, args) =>
                {
                    _ack = false;
                };

                channel.ConfirmSelect();

                while (!_stopRequested)
                {
                    while (!_pendingNotifications.IsEmpty)
                    {
                        NotificationDto dto;
                        if (!_pendingNotifications.TryPeek(out dto))
                            break;

                        try
                        {
                            _ack = false;
                            channel.BasicPublish(exchange: Settings.RmqExchangeName, routingKey: "", basicProperties: null, body: ProtoBufHelper.Serialize(dto));
                            channel.WaitForConfirms(TimeSpan.FromSeconds(30));

                            if (_ack)
                            {
                                _log.IndentInfo("Notification sent: {0}", dto);
                                _pendingNotifications.TryDequeue(out dto);
                            }
                            else
                                _log.IndentError("Unable to publish notification: {0}", dto);
                        }
                        catch (Exception e)
                        {
                            _log.IndentErrorWithException(e, "Unable to publish notification: {0}", dto);
                            break;
                        }
                    }

                    Thread.Sleep(200);
                }
                _threadEvent.Set();
            }
        }
    }
}
