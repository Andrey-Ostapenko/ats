using System;

namespace AccountingTransactionService.DbEntities
{
    public class PaymentStatistics
    {
        public PaymentStatistics() { }

        public DateTime PaymentDate
        {
            set;
            get;
        }

        public string PointId
        {
            set;
            get;
        }

        public string PaymentName
        {
            set;
            get;
        }

        public int Count
        {
            set;
            get;
        }

        public decimal Amount
        {
            set;
            get;
        }

        public decimal CommissionAmount
        {
            set;
            get;
        }
    }
}