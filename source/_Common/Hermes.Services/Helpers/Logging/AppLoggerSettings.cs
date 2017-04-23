using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Hermes.Services.Helpers.Logging
{
    public class AppLoggerSettings
    {
        /* Public */
        public string IndentSymbol { get; private set; }
        public string CurrentIndentString { get; private set; }

        public Dictionary<string, object> CustomFields { get; private set; }

        public int IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                _indentLevel = value;
                UpdateIndentString();
            }
        }

        /* Private */
        private int _indentLevel = 0;
        private Int64 _msgId = 0;

        public AppLoggerSettings(string indentSymbol, Dictionary<string, object> customFields)
        {
            IndentSymbol = indentSymbol;
            CurrentIndentString = "";

            CustomFields = customFields;
        }

        public AppLoggerSettings()
            : this ("--", new Dictionary<string, object>())
        {
        }

        public void LoadCache(string fileName)
        {
            if (File.Exists(fileName))
                _msgId = int.Parse(File.ReadAllText(fileName));
        }

        public void SaveCache(string fileName)
        {
            File.WriteAllText(fileName, _msgId.ToString(CultureInfo.InvariantCulture));
        }

        public Int64 GetMessageId()
        {
            return _msgId++;
        }

        private void UpdateIndentString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < IndentLevel; ++i)
                sb.Append(IndentSymbol);

            if (sb.Length > 0)
                sb.Append(" ");

            CurrentIndentString = sb.ToString();
        }
    }
}