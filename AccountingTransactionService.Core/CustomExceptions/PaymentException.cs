using System;

namespace AccountingTransactionService.CustomExceptions
{
    public class PaymentException : Exception
    {
        private readonly long _paymentId;
        private readonly string[] _additionalData;

        public PaymentException(long paymentId, string message)
            : base(message)
        {
            this._paymentId = paymentId;
            this._additionalData = null;
        }

        public PaymentException(long paymentId, string message, string[] additionalData)
            : base(message)
        {
            this._paymentId = paymentId;
            this._additionalData = additionalData;
        }

        public long PaymentId => _paymentId;

        public string[] AdditionalData => _additionalData;
    }
}
