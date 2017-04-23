using System;
using JetBrains.Annotations;
using NLog;

namespace Hermes.Services.Helpers.Logging
{
    public class IndentBlock : IDisposable
    {
        private readonly int _initialIndentLevel;
        private readonly AppLoggerSettings _settings;

        [StringFormatMethod("messageFormat")]
        public IndentBlock(AppLogger log, AppLoggerSettings settings, LogLevel level, string messageFormat, params object[] args)
        {
            _settings = settings;
            _initialIndentLevel = _settings.IndentLevel;

            log.IndentLog(level, messageFormat, args);
            log.Indent();
        }

        public void Dispose()
        {
            // Reset indent level
            _settings.IndentLevel = _initialIndentLevel;
        }
    }
}