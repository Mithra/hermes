using System;
using Nancy.Hosting.Self;
using NLog;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace Hermes.Server
{
    public class HermesService
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private NancyHost _nancyHost;
        private IBootstrap _websocketServer;

        public bool Start()
        {
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

                _log.Info("Starting Websockets server");
                StartResult result = _websocketServer.Start();
                _log.Info(" Result: {0}", result);

                if (result == StartResult.Failed)
                {
                    _log.Error("Unable to start Websockets server");
                    return false;
                }

                _log.Info("Creating Nancy API server");
                _nancyHost = new NancyHost(new Uri(Settings.SelfHostUrl));

                _log.Info("Starting Nancy API server");
                _nancyHost.Start();

                return true;
            }
            catch (Exception e)
            {
                _log.Error(e, "Unable to start HermesService");

                return false;
            }
        }

        public void Stop()
        {
            _nancyHost.Stop();
            _websocketServer.Stop();

            _log.Info("Stopped");
        }
    }
}
