using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class PaymentStatisticsPeriodReadException : Exception
    {
        public PaymentStatisticsPeriodReadException()
        {
        }

        public PaymentStatisticsPeriodReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
