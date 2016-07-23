using AccountingTransactionService.DbEntities;
using AccountingTransactionService.Interfaces;

namespace AccountingTransactionService.Gateway
{
    class DefaultGatewayProvider : IGatewayProvider
    {
        public void SendAccountingTransactions(AccountingTransactionCollection accTransactions)
        {
        }

        public void SendPaymentStatistics(PaymentStatisticsCollection payStatistics)
        {
        }
    }
}
