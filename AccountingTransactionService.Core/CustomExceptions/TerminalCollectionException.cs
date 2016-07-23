using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class TerminalCollectionException : Exception
    {
        private readonly long _collectionId;
        private readonly string[] _additionalData;

        public TerminalCollectionException(long collectionId, string message)
            : base(message)
        {
            this._collectionId = collectionId;
            this._additionalData = null;
        }

        public TerminalCollectionException(long collectionId, string message, string[] additionalData)
            : base(message)
        {
            this._collectionId = collectionId;
            this._additionalData = additionalData;
        }

        public long CollectionId => _collectionId;

        public string[] AdditionalData => _additionalData;
    }
}
