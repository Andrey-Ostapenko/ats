using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionsSaveToFsException : Exception
    {
        public AccountingTransactionsSaveToFsException()
        {
        }

        public AccountingTransactionsSaveToFsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
