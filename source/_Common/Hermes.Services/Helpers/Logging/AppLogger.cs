using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NLog;

namespace Hermes.Services.Helpers.Logging
{
    /// <summary>
    /// AppLogger is a simple wrapper around an NLog logger instance providing ways to indent logs for
    /// easier debugging.
    /// </summary>
    public class AppLogger
    {
        public AppLoggerSettings Settings { get; private set; }
        private readonly Logger _log;
        
        public AppLogger(AppLoggerSettings settings, Logger nLogger)
        {
            Settings = settings;
            _log = nLogger;
        }

        public void Indent(int delta = 2)
        {
            Settings.IndentLevel += delta;
        }

        public void Unindent(int delta = -2)
        {
            Settings.IndentLevel += delta;
        }

        public void ResetIndent()
        {
            Settings.IndentLevel = 0;
        }
        
        // Debug
        [StringFormatMethod("messageFormat")]
        public void Debug(string messageFormat, params object[] args)
        {
            Log(LogLevel.Debug, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void IndentDebug(string messageFormat, params object[] args)
        {
            IndentLog(LogLevel.Debug, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void InlineIndentDebug(string messageFormat, params object[] args)
        {
            InlineIndentLog(LogLevel.Debug, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public IndentBlock IndentBlockDebug(string messageFormat, params object[] args)
        {
            return new IndentBlock(this, Settings, LogLevel.Debug, messageFormat, args);
        }
        
        // Info
        [StringFormatMethod("messageFormat")]
        public void Info(string messageFormat, params object[] args)
        {
            Log(LogLevel.Info, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void IndentInfo(string messageFormat, params object[] args)
        {
            IndentLog(LogLevel.Info, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void InlineIndentInfo(string messageFormat, params object[] args)
        {
            InlineIndentLog(LogLevel.Info, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public IndentBlock IndentBlockInfo(string messageFormat, params object[] args)
        {
            return new IndentBlock(this, Settings, LogLevel.Info, messageFormat, args);
        }

        // Warn
        [StringFormatMethod("messageFormat")]
        public void Warn(string messageFormat, params object[] args)
        {
            Log(LogLevel.Warn, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void IndentWarn(string messageFormat, params object[] args)
        {
            IndentLog(LogLevel.Warn, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void InlineIndentWarn(string messageFormat, params object[] args)
        {
            InlineIndentLog(LogLevel.Warn, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public IndentBlock IndentBlockWarn(string messageFormat, params object[] args)
        {
            return new IndentBlock(this, Settings, LogLevel.Warn, messageFormat, args);
        }

        // Error
        [StringFormatMethod("messageFormat")]
        public void Error(string messageFormat, params object[] args)
        {
            Log(LogLevel.Error, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void IndentError(string messageFormat, params object[] args)
        {
            IndentLog(LogLevel.Error, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void InlineIndentError(string messageFormat, params object[] args)
        {
            InlineIndentLog(LogLevel.Error, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public IndentBlock IndentBlockError(string messageFormat, params object[] args)
        {
            return new IndentBlock(this, Settings, LogLevel.Error, messageFormat, args);
        }

        // Error (with exception)
        [StringFormatMethod("messageFormat")]
        public void ErrorWithException(Exception exception, string messageFormat, params object[] args)
        {
            LogWithException(LogLevel.Error, exception, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void IndentErrorWithException(Exception exception, string messageFormat, params object[] args)
        {
            IndentLogWithException(LogLevel.Error, exception, messageFormat, args);
        }

        [StringFormatMethod("messageFormat")]
        public void InlineErrorWithException(Exception exception, string messageFormat, params object[] args)
        {
            InlineIndentLog(LogLevel.Error, messageFormat, args);
        }

        // Generic
        [StringFormatMethod("messageFormat")]
        public void Log(LogLevel level, string messageFormat, params object[] args)
        {
            Log(level, String.Format(messageFormat, args));
        }

        [StringFormatMethod("messageFormat")]
        public void LogWithException(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            Log(level, String.Format(messageFormat, args), exception);
        }

        [StringFormatMethod("messageFormat")]
        public void IndentLog(LogLevel level, string messageFormat, params object[] args)
        {
            try
            {
                string msg = String.Format(messageFormat, args);
                Log(level, String.Format("{0}{1}", Settings.CurrentIndentString, msg));
            }
            catch (Exception e)
            {
                string securedFormat = messageFormat.Replace("{", "{{").Replace("}", "}}");
                IndentLogWithException(level, e, "Unable to log the following entry (invalid number of parameters provided): '{0}'", securedFormat);
            }
        }

        [StringFormatMethod("messageFormat")]
        public void IndentLogWithException(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            try
            {
                string msg = String.Format(messageFormat, args);
                Log(level, String.Format("{0}{1}", Settings.CurrentIndentString, msg), exception);
            }
            catch (Exception e)
            {
                string securedFormat = messageFormat.Replace("{", "{{").Replace("}", "}}");
                IndentLogWithException(level, e, "Unable to log the following entry (invalid number of parameters provided): '{0}'", securedFormat);
            }
        }

        [StringFormatMethod("messageFormat")]
        public void InlineIndentLog(LogLevel level, string messageFormat, params object[] args)
        {
            Indent();
            IndentLog(level, messageFormat, args);
            Unindent();
        }

        [StringFormatMethod("messageFormat")]
        public void InlineIndentLogWithException(LogLevel level, Exception exception, string messageFormat, params object[] args)
        {
            Indent();
            IndentLogWithException(level, exception, messageFormat, args);
            Unindent();
        }

        public void Log(LogLevel level, string message, Exception exception = null)
        {
            var eventInfo = new LogEventInfo
            {
                Message = message,
                Level = level,
                Exception = exception,
                LoggerName = _log.Name,
                TimeStamp = DateTime.UtcNow
            };
            
            // Add custom fields
            if (Settings.CustomFields != null)
                foreach (KeyValuePair<string, object> customField in Settings.CustomFields)
                    eventInfo.Properties.Add(customField.Key, customField.Value);

            // Add message ID
            eventInfo.Properties.Add("messageid", Settings.GetMessageId());

            _log.Log(eventInfo);
        }
    }
}
