using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionsReadException : Exception
    {
        public AccountingTransactionsReadException()
        {
        }

        public AccountingTransactionsReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
