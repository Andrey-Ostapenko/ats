using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class PaymentStatisticsEventReadException : Exception
    {
        public PaymentStatisticsEventReadException()
        {
        }

        public PaymentStatisticsEventReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
