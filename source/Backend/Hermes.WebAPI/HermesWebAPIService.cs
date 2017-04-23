using System;
using Hermes.DataObjects.Notification;
using Hermes.Services;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Logging;
using Hermes.WebAPI.Rmq;
using Nancy.Hosting.Self;
using Nancy.TinyIoc;
using NLog;

namespace Hermes.WebAPI
{
    public class HermesWebAPIService
    {
        private AppLogger _log;
        private AppLoggerSettings _logSettings;

        private NancyHost _nancyHost;
        private RmqNotifier _rmqNotifier;

        public bool Start()
        {
            _logSettings = new AppLoggerSettings();

            GlobalAppLogger.Init(_logSettings);
            _log = GlobalAppLogger.GetLogger(LogManager.GetCurrentClassLogger());

            try
            {
                _log.Info("--------------------------------------------------");
                _log.Info("Creating RMQ notifier");
                _rmqNotifier = new RmqNotifier();
                TinyIoCContainer.Current.Register<INotificationListener>(_rmqNotifier);

                _log.Info("Creating Nancy API server");
                _nancyHost = new NancyHost(new Uri(Settings.SelfHostUrl));

                ProtoBufHelper.RegisterType(typeof(NotificationDto));

                _log.Info("Starting RMQ notifier");
                _rmqNotifier.Start();

                _log.Info("Starting Nancy API server");
                _nancyHost.Start();

                return true;
            }
            catch (Exception e)
            {
                _log.ErrorWithException(e, "Unable to start HermesService");

                return false;
            }
        }

        public void Stop()
        {
            _log.Info("Requesting stop");

            _nancyHost.Stop();
            _rmqNotifier.Stop();

            _log.Info("Stopped");
        }
    }
}
