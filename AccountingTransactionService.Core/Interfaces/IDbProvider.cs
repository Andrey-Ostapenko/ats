using System;
using AccountingTransactionService.DbEntities;

namespace AccountingTransactionService.Interfaces
{
    public interface IDbProvider
    {
        #region Операции над событиями для формирования проводок

        EventPeriod GetLastAccountingTransactionEventPeriod();
        AccountingTransactionEventCollection GetAccountingTransactionEvents();
        void InsertAccountingTransactionEvent(AccountingTransactionEvent accTranEvent);
        void UpdateAccountingTransactionEvent(AccountingTransactionEvent accTranEvent);

        #endregion Операции над событиями для формирования проводок

        #region Операции над событиями для формирования статистики по платежам

        EventPeriod GetLastPaymentStatisticsEventPeriod();
        PaymentStatisticsEventCollection GetPaymentStatisticsEvents();
        void InsertPaymentStatisticsEvent(PaymentStatisticsEvent payStatEvent);
        void UpdatePaymentStatisticsEvent(PaymentStatisticsEvent payStatEvent);

        #endregion Операции над событиями для формирования статистики по платежам

        #region Получение исходных данных о реквизитах платежа

        Payment GetPayment(int pointId, string sessionNumber);
        Payment GetRequsites(long providerId, int pointId);

        #endregion Получение исходных данных о реквизитах платежа

        #region Получение исходных данных о платежах, инкассациях и счетах и комиссиях

        Payment GetOtpPayment(AccountingTransactionEvent accTranEvent);
        PaymentCollection GetPayments(AccountingTransactionEvent accTranEvent);
        PaymentStatisticsCollection GetPaymentStatistics(PaymentStatisticsEvent payStatEvent);
        TerminalCollection GetTerminalCollection(AccountingTransactionEvent accTranEvent);
        AccountBindingCollection GetAccountBindings();
        ReplacementAccountCollection GetReplacementAccounts();
        decimal GetIntegratorCommission(long gatewayId, long providerId, DateTime paymentDate, decimal amount, decimal clientCommission);

        #endregion Получение исходных данных о платежах, инкассациях и счетах и комиссиях

        #region Чтение настроек для формирования проводок

        AccountingTransactionTemplateDetailCollection GetAccountingTransactionShemes();
        AccountSearchRuleCollection GetAccountSearchRules();
        
        #endregion Чтение настроек для формирования проводок

        #region Операции над сформированными проводками

        AccountingTransactionCollection GetAccountingTransactions(AccountingTransactionEvent accTranEvent);
        void InsertAccountingTransactions(AccountingTransactionEvent accTranEvent, AccountingTransactionCollection accountingTransactions);

        #endregion Операции над сформированными проводками
    }
}
