using System;
using System.Configuration;
using System.Reflection;

namespace Hermes.Services
{
    public static class Settings
    {
        // Service
        public static string ServiceName { get; private set; }
        public static string ServiceDisplayName { get; private set; }
        public static string ServiceDescription { get; private set; }
        public static string SelfHostUrl { get; set; }

        // RabbitMQ
        public static string RmqHost { get; private set; }
        public static int RmqPort { get; private set; }
        public static string RmqUsername { get; private set; }
        public static string RmqPassword { get; private set; }
        public static string RmqExchangeName { get; private set; }

        public static void LoadSettings()
        {
            foreach (var propertyInfo in typeof(Settings).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.FlattenHierarchy))
            {
                if (propertyInfo.DeclaringType == null)
                    continue;

                var actualProperty = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);

                bool propertySet = false;

                string value = ConfigurationManager.AppSettings[propertyInfo.Name];
                if (!propertySet && value != null)
                {
                    if (actualProperty.PropertyType.IsEnum)
                        actualProperty.SetValue(null, Enum.Parse(actualProperty.PropertyType, value));
                    else
                        actualProperty.SetValue(null, Convert.ChangeType(value, actualProperty.PropertyType));
                }
            }
        }
    }
}
