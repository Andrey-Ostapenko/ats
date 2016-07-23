using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionsSaveToAbsException : Exception
    {
        public AccountingTransactionsSaveToAbsException()
        {
        }

        public AccountingTransactionsSaveToAbsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
