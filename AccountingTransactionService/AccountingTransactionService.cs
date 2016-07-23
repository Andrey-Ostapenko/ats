using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using AccountingTransactionService.Configuration;
using AccountingTransactionService.Interfaces;
using Utilities.Logging;

namespace AccountingTransactionService
{
    public partial class AccountingTransactionService : ServiceBase
    {
        private ServiceHost _attributesService;
        private AccountingTransactionEventListener _accTranEventListener;
        private PaymentStatisticsEventListener _payStatEventListener;

        public AccountingTransactionService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.WriteToFile("Запуск сервиса...");

                Assembly dbProviderAssembly = Assembly.LoadWithPartialName(ConfigurationProvider.DbProviderSettings.Assembly);
                IDbProvider dbProvider = dbProviderAssembly.CreateInstance(ConfigurationProvider.DbProviderSettings.Type, true) as IDbProvider;

                Assembly gatewayProviderAssembly = Assembly.LoadWithPartialName(ConfigurationProvider.GatewayProviderSettings.Assembly);
                IGatewayProvider gatewayProvider = gatewayProviderAssembly.CreateInstance(ConfigurationProvider.GatewayProviderSettings.Type, true) as IGatewayProvider;

                if (ConfigurationProvider.AttributesService)
                {
                    _attributesService = new ServiceHost(new AttributesService(dbProvider));
                    _attributesService.Open();
                }

                object syncObject = new object();

                if (ConfigurationProvider.AccountingTransactionService)
                {                    
                    _accTranEventListener = new AccountingTransactionEventListener(syncObject, dbProvider, gatewayProvider);
                    _accTranEventListener.Start();                    
                }

                if (ConfigurationProvider.PaymentStatisticsService)
                {
                    _payStatEventListener = new PaymentStatisticsEventListener(syncObject, dbProvider, gatewayProvider);
                    _payStatEventListener.Start();
                }

                Log.WriteToFile(string.Format("AccountingTransactionService={0}.", ConfigurationProvider.AccountingTransactionService));
                Log.WriteToFile(string.Format("PaymentStatisticsService={0}.", ConfigurationProvider.PaymentStatisticsService));
                Log.WriteToFile(string.Format("AttributesService={0}.", ConfigurationProvider.AttributesService));
                Log.WriteToFile("Cервис стартовал успешно.");
            }
            catch (Exception ex)
            {
                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }

                try
                {
                    Stop();
                }
                catch
                {
                }
            }

        }

        protected override void OnStop()
        {
            try
            {
                Log.WriteToFile("Остановка сервиса...");                
                
                if (_attributesService != null && _attributesService.State == CommunicationState.Opened)
                {
                    _attributesService.Close();
                }

                if (_accTranEventListener != null) _accTranEventListener.Stop();

                if (_payStatEventListener != null) _payStatEventListener.Stop();

                Log.WriteToFile("Сервис остановлен успешно.");
            }
            catch (Exception ex)
            {
                try
                {
                    Log.WriteToFile(ex);
                }
                catch
                {
                }
            }
        }
    }
}
