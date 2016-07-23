using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class AccountingTransactionsException : Exception
    {
        public AccountingTransactionsException()
        {
        }

        public AccountingTransactionsException(string message)
            : base(message)
        {
        }
    }
}
