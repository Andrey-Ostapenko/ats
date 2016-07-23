using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionEventReadException : Exception
    {
        public AccountingTransactionEventReadException()
        {
        }

        public AccountingTransactionEventReadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }   
}
