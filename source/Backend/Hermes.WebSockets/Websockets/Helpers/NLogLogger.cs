using System;
using NLog;
using SuperSocket.SocketBase.Logging;

namespace Hermes.WebSockets.Websockets.Helpers
{
    public class NLogLogger : ILog
    {
        public bool IsDebugEnabled { get { return _log.IsDebugEnabled; } }
        public bool IsErrorEnabled { get { return _log.IsErrorEnabled; } }
        public bool IsFatalEnabled { get { return _log.IsFatalEnabled; } }
        public bool IsInfoEnabled { get { return _log.IsInfoEnabled; } }
        public bool IsWarnEnabled { get { return _log.IsWarnEnabled; } }

        private readonly Logger _log;

        public NLogLogger(Logger log)
        {
            _log = log;
        }

        public void Debug(object message)
        {
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _log.Debug(message);
        }

        public void DebugFormat(string format, object arg0)
        {
            _log.Debug(format, arg0);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _log.Debug(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.Debug(provider, format, args);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            _log.Debug(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.Debug(format, arg0, arg1, arg2);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message);
        }

        public void ErrorFormat(string format, object arg0)
        {
            _log.Error(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.Error(format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.Error(provider, format, args);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            _log.Error(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.Error(format, arg0, arg1, arg2);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message);
        }

        public void FatalFormat(string format, object arg0)
        {
            _log.Fatal(format, arg0);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.Fatal(format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.Fatal(provider, format, args);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            _log.Fatal(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.Fatal(format, arg0, arg1, arg2);
        }

        public void Info(object message)
        {
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _log.Info(message);
        }

        public void InfoFormat(string format, object arg0)
        {
            _log.Info(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _log.Info(format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.Info(provider, format, args);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            _log.Info(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.Info(format, arg0, arg1, arg2);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message);
        }

        public void WarnFormat(string format, object arg0)
        {
            _log.Warn(format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.Warn(format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _log.Warn(provider, format, args);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            _log.Warn(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            _log.Warn(format, arg0, arg1, arg2);
        }
    }
}
