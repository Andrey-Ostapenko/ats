using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionEventStatusUpdateException : Exception
    {
        public AccountingTransactionEventStatusUpdateException()
        {
        }

        public AccountingTransactionEventStatusUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
