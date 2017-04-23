using System;
using Hermes.DataObjects.Notification;
using Hermes.Services.Helpers;
using Hermes.Services.Helpers.Logging;
using NLog;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace Hermes.WebSockets
{
    public class HermesWebSocketService
    {
        private AppLogger _log;
        private AppLoggerSettings _logSettings;

        private IBootstrap _websocketServer;

        public bool Start()
        {
            _logSettings = new AppLoggerSettings();

            GlobalAppLogger.Init(_logSettings);
            _log = GlobalAppLogger.GetLogger(LogManager.GetCurrentClassLogger());

            try
            {
                _log.Info("--------------------------------------------------");
                _log.Info("Creating Websockets server");
                _websocketServer = BootstrapFactory.CreateBootstrap();

                if (!_websocketServer.Initialize())
                {
                    _log.Error("Unable to initialize Websockets server");
                    return false;
                }

                ProtoBufHelper.RegisterType(typeof(NotificationDto));

                _log.Info("Starting Websockets server");
                StartResult result = _websocketServer.Start();
                _log.Info(" Result: {0}", result);

                if (result == StartResult.Failed)
                {
                    _log.Error("Unable to start Websockets server");
                    return false;
                }

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
            _websocketServer.Stop();

            _log.Info("Stopped");
        }
    }
}
