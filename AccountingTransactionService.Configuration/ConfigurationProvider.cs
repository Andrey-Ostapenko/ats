using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using AccountingTransactionService.Configuration.Entities;
using AccountingTransactionService.Configuration.GatewayConfigurations;

namespace AccountingTransactionService.Configuration
{
    public static class ConfigurationProvider
    {
        private static Hashtable servicesSettings;
        private static Hashtable dbProviderSettings;
        private static Hashtable gatewaySettings;
        private static Dictionary<string, string> sqlQueries;
        private static CftGatewayConfiguration cftGatewayConfiguration;
        private static OtpGatewayConfiguration otpGatewayConfiguration;
        private static VbGatewayConfiguration vbGatewayConfiguration;

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
            DefaultEntityTypeId = int.Parse(ConfigurationManager.AppSettings["DefaultEntityTypeId"]);
            DefaultEntityId = int.Parse(ConfigurationManager.AppSettings["DefaultEntityId"]);
            DefaultPointId = int.Parse(ConfigurationManager.AppSettings["DefaultPointId"]);
            DefaultData = int.Parse(ConfigurationManager.AppSettings["DefaultData"]);
            MainBank = int.Parse(ConfigurationManager.AppSettings["MainBank"]);
            OtpCommissionTemplateId = int.Parse(ConfigurationManager.AppSettings["OtpCommissionTemplateId"]);
            OtpCabinetGatewayId = long.Parse(ConfigurationManager.AppSettings["OtpCabinetGatewayId"]);
            VbUser = ConfigurationManager.AppSettings["VbUser"];
            OperatorComposite = 1;
            sqlQueries = new Dictionary<string, string>();
            dbProviderSettings = ConfigurationManager.GetSection("dbProviderSettings") as Hashtable;
        }

        private static bool getStartFlag(string key)
        {
            return servicesSettings.ContainsKey(key) ? bool.Parse(servicesSettings[key].ToString().ToLower()) : false;
        }

        private static string getSqlText(string key)
        {
            if (!sqlQueries.ContainsKey(key))
            {
                string path = Path.Combine(Path.GetDirectoryName(Environment.CommandLine.Replace("\"", string.Empty).Trim()), key);
                sqlQueries.Add(key, File.ReadAllText(path));
            }

            return sqlQueries[key];
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
        }

        public static GatewayProviderSettings GatewayProviderSettings
        {
            private set;
            get;
        }

        public static int PollTimerInterval
        {
            private set;
            get;
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

        public static int DefaultEntityTypeId
        {
            private set;
            get;
        }

        public static int DefaultEntityId
        {
            private set;
            get;
        }

        public static int DefaultPointId
        {
            private set;
            get;
        }

        public static int DefaultData
        {
            private set;
            get;
        }

        public static int MainBank
        {
            private set;
            get;
        }

        public static int OperatorComposite
        {
            private set;
            get;
        }

        public static long OtpCabinetGatewayId
        {
            private set;
            get;
        }

        public static int OtpCommissionTemplateId
        {
            private set;
            get;
        }

        public static string VbUser
        {
            private set;
            get;
        }

        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["FS"].ConnectionString;
            }
        }

        public static string SqlSelectLastAccountingTransactionEventPeriod
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectLastAccountingTransactionEventPeriod"].ToString());
            }
        }

        public static string SqlSelectLastPaymentStatisticsEventPeriod
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectLastPaymentStatisticsEventPeriod"].ToString());
            }
        }

        public static string SqlSelectPayment
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectPayment"].ToString());
            }
        }

        public static string SqlSelectRequisites
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectRequisites"].ToString());
            }
        }

        public static string SqlSelectPayments
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectPayments"].ToString());
            }
        }

        public static string SqlSelectOtpPayment
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectOtpPayment"].ToString());
            }
        }

        public static string SqlSelectPointPayments
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectPointPayments"].ToString());
            }
        }

        public static string SqlSelectAccountingTransactionShemes
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectAccountingTransactionShemes"].ToString());
            }
        }

        public static string SqlSelectAccountSearchRules
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectAccountSearchRules"].ToString());
            }
        }

        public static string SqlSelectReplacementAccounts
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectReplacementAccounts"].ToString());
            }
        }

        public static string SqlSelectAccountBindings
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectAccountBindings"].ToString());
            }
        }

        public static string SqlSelectAccountingTransactionEvents
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectAccountingTransactionEvents"].ToString());
            }
        }

        public static string SqlSelectPaymentStatisticsEvents
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectPaymentStatisticsEvents"].ToString());
            }
        }

        public static string SqlSelectAccountingTransactions
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectAccountingTransactions"].ToString());
            }
        }

        public static string SqlSelectTerminalCollection
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectTerminalCollection"].ToString());
            }
        }


        public static string SqlSelectPaymentStatistics
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectPaymentStatistics"].ToString());
            }
        }

        public static string SqlSelectAccountingTransactionFields
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlSelectAccountingTransactionFields"].ToString());
            }
        }

        public static string SqlInsertAccountingTransaction
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlInsertAccountingTransaction"].ToString());
            }
        }

        public static string SqlInsertAccountingTransactionPayment
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlInsertAccountingTransactionPayment"].ToString());
            }
        }

        public static string SqlInsertAccountingTransactionField
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlInsertAccountingTransactionField"].ToString());
            }
        }

        public static string SqlInsertAccountingTransactionEvent
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlInsertAccountingTransactionEvent"].ToString());
            }
        }

        public static string SqlInsertPaymentStatisticsEvent
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlInsertPaymentStatisticsEvent"].ToString());
            }
        }

        public static string SqlUpdateAccountingTransactionEvent
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlUpdateAccountingTransactionEvent"].ToString());
            }
        }

        public static string SqlUpdatePaymentStatisticsEvent
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlUpdatePaymentStatisticsEvent"].ToString());
            }
        }

        public static string SqlGetIntegratorCommission
        {
            get
            {
                return getSqlText(dbProviderSettings["SqlGetIntegratorCommission"].ToString());
            }
        }

        public static CftGatewayConfiguration CftGatewayConfiguration
        {
            get
            {
                if (cftGatewayConfiguration == null) cftGatewayConfiguration = new CftGatewayConfiguration();
                return cftGatewayConfiguration;
            }
        }

        public static OtpGatewayConfiguration OtpGatewayConfiguration
        {
            get
            {
                if (otpGatewayConfiguration == null) otpGatewayConfiguration = new OtpGatewayConfiguration();
                return otpGatewayConfiguration;
            }
        }

        public static VbGatewayConfiguration VbGatewayConfiguration
        {
            get
            {
                if (vbGatewayConfiguration == null) vbGatewayConfiguration = new VbGatewayConfiguration();
                return vbGatewayConfiguration;
            }
        }
    }
}
