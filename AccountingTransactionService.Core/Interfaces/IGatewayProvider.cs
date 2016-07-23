using AccountingTransactionService.DbEntities;

namespace AccountingTransactionService.Interfaces
{
    public interface IGatewayProvider
    {
        void SendAccountingTransactions(AccountingTransactionCollection accTransactions);
        void SendPaymentStatistics(PaymentStatisticsCollection payStatistics);
    }    
}
