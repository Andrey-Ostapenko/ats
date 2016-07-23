using System;

namespace AccountingTransactionService.CustomExceptions
{
    class SubdealerAttributesException : Exception
    {
        public SubdealerAttributesException()
        {
        }

        public SubdealerAttributesException(string message)
            : base(message) { }
    }
}
