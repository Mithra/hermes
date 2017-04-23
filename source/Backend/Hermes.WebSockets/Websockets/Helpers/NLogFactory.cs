using NLog;
using SuperSocket.SocketBase.Logging;

namespace Hermes.WebSockets.Websockets.Helpers
{
    public class NLogFactory : ILogFactory
    {
        public ILog GetLog(string name)
        {
            return new NLogLogger(LogManager.GetLogger(name));
        }
    }
}
