using System;

namespace AccountingTransactionService.CustomExceptions
{
    class PymentNotExistException : Exception
    {
        public PymentNotExistException()
        {
        }

        public PymentNotExistException(string message)
            : base(message)
        {
        }
    }
}
