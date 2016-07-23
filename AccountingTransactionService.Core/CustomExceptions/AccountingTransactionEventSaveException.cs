using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionEventSaveException : Exception
    {
        public AccountingTransactionEventSaveException()
        {
        }

        public AccountingTransactionEventSaveException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
