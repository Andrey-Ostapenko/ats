using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class PaymentStatisticsEventStatusUpdateException : Exception
    {
        public PaymentStatisticsEventStatusUpdateException()
        {
        }

        public PaymentStatisticsEventStatusUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
