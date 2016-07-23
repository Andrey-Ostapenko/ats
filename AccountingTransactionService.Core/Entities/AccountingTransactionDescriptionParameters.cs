using System;

namespace AccountingTransactionService.Entities
{  
    public class AccountingTransactionDescriptionParameters
    {
        private DateTime paymentDateFrom;
        private DateTime paymentDateTo;

        public AccountingTransactionDescriptionParameters()
        {
            paymentDateFrom = DateTime.MaxValue;
            paymentDateTo = DateTime.MinValue;
        }

        public decimal AmountAll
        {
            set;
            get;
        }

        public decimal RewardAmount
        {
            set;
            get;
        }

        public DateTime PaymentDateFrom
        {
            set
            {
                if (paymentDateFrom > value) paymentDateFrom = value;
            }

            get
            {
                return paymentDateFrom;
            }
        }

        public DateTime PaymentDateTo
        {
            set
            {
                if (paymentDateTo < value) paymentDateTo = value;
            }

            get
            {
                return paymentDateTo;
            }
        }
    }
}
