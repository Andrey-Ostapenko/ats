using System;

namespace AccountingTransactionService.CustomExceptions
{
    class RecipientAttributesException : Exception
    {
        public RecipientAttributesException() { }

        public RecipientAttributesException(string message)
            : base(message) { }
    }
}
