using System;
using System.Collections.Generic;
using Hermes.Services.Helpers.Logging;
using NLog;

namespace Hermes.Services.Helpers
{
    public static class LoggerHelper
    {
        public const string HermesWebApi = "hermes-webapi";
        public const string HermesWebSockets = "hermes-websockets";

        public static AppLogger GetLogger(string applicationName, string callName)
        {
            return new AppLogger(new AppLoggerSettings("--", new Dictionary<string, object>
            {
                { "call-id", Guid.NewGuid() },
                { "call-name", callName }
            }), LogManager.GetCurrentClassLogger());
        }
    }
}
