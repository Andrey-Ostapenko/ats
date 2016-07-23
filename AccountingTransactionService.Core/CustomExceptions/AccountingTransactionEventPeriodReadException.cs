using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionEventPeriodReadException : Exception
    {
        public AccountingTransactionEventPeriodReadException()
        {
        }

        public AccountingTransactionEventPeriodReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
