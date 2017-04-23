using System;
using NLog;

namespace Hermes.Services.Helpers.Logging
{
    /// <summary>
    /// GlobalAppLogger is a static wrapper around one instance of AppLoggerSettings.
    /// This allows a shared indentation level across the entire code.
    /// </summary>
    public static class GlobalAppLogger
    {
        private static AppLoggerSettings _settings = null;

        public static void Init(AppLoggerSettings settings)
        {
            _settings = settings;
        }

        public static AppLogger GetLogger(Logger nLogger)
        {
            if (_settings == null)
                throw new Exception("You need to call AppLoggerSettings.Init() before creating a logger with it !");

            return new AppLogger(_settings, nLogger);
        }
    }
}