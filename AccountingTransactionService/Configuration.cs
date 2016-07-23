using System;
using System.Configuration;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace AccountingTransactionService.Configuration
{
    static class ConfigurationProvider
    {
        //private static DbProviderSettings dbProviderSettings;
        //private static GatewayProviderSettings gatewayProviderSettings;
        //private static int pollTimerInterval;
        private static Hashtable servicesSettings;

        static ConfigurationProvider()
        {
            ModulesSettings modulesSettings = ConfigurationManager.GetSection("modules") as ModulesSettings;
            servicesSettings = ConfigurationManager.GetSection("services") as Hashtable;
            DbProviderSettings = modulesSettings.DbProviderSettings;
            GatewayProviderSettings = modulesSettings.GatewayProviderSettings;
            PollTimerInterval = int.Parse(ConfigurationManager.AppSettings["PollTimerInterval"]);
            DayCloseEvent = bool.Parse(ConfigurationManager.AppSettings["DayCloseEvent"]);
            DateTime time = DateTime.Parse(ConfigurationManager.AppSettings["DayCloseTime"]);
            DayCloseTime = new TimeSpan(time.Hour, time.Minute, time.Second);
        }

        private static bool getStartFlag(string key)
        {
            return servicesSettings.ContainsKey(key) ? bool.Parse(servicesSettings[key].ToString().ToLower()) : false;
        }

        public static bool AttributesService
        {
            get { return getStartFlag("AttributesService"); }
        }

        public static bool AccountingTransactionService
        {
            get { return getStartFlag("AccountingTransactionService"); }
        }

        public static bool PaymentStatisticsService
        {
            get { return getStartFlag("PaymentStatisticsService"); }
        }

        public static DbProviderSettings DbProviderSettings
        {
            private set;
            get;
            //get { return dbProviderSettings; }
        }

        public static GatewayProviderSettings GatewayProviderSettings
        {
            private set;
            get;
            //get { return gatewayProviderSettings; }
        }

        public static int PollTimerInterval
        {
            private set;
            get;
            //get { return pollTimerInterval; }
        }

        public static bool DayCloseEvent
        {
            private set;
            get;
        }

        public static TimeSpan DayCloseTime
        {
            private set;
            get;
        }

        public static bool Logging
        {
            get { return true; }
        }
    }

    public class AccountingTransactionServiceSettingsSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModulesSettings));
            XmlReader reader = new XmlNodeReader(section);
            return serializer.Deserialize(reader);
        }
    }

    [XmlRoot("modules")]
    public class ModulesSettings
    {
        [XmlElement("dbProvider")]
        public DbProviderSettings DbProviderSettings
        {
            set;
            get;
        }

        [XmlElement("gatewayProvider")]
        public GatewayProviderSettings GatewayProviderSettings
        {
            set;
            get;
        }
    }

    public class DbProviderSettings
    {
        [XmlAttribute("assembly")]
        public string Assembly
        {
            set;
            get;
        }

        [XmlAttribute("type")]
        public string Type
        {
            set;
            get;
        }
    }

    public class GatewayProviderSettings
    {
        [XmlAttribute("assembly")]
        public string Assembly
        {
            set;
            get;
        }

        [XmlAttribute("type")]
        public string Type
        {
            set;
            get;
        }
    }
}
