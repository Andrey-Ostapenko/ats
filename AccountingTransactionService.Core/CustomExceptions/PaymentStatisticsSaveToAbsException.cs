using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class PaymentStatisticsSaveToAbsException : Exception
    {
        public PaymentStatisticsSaveToAbsException()
        {
        }

        public PaymentStatisticsSaveToAbsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
