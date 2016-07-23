using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class PaymentStatisticsEventSaveException : Exception
    {
        public PaymentStatisticsEventSaveException()
        {
        }

        public PaymentStatisticsEventSaveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
