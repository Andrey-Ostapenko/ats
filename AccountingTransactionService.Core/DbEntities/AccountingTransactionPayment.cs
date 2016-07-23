namespace AccountingTransactionService.DbEntities
{
    public class AccountingTransactionPayment
    {
        public AccountingTransactionPayment() { }

        public AccountingTransactionPayment(
            long id,
            decimal amount)
        {
            Id = id;
            Amount = amount;
        }

        public long Id
        {
            set;
            get;
        }

        public decimal Amount
        {
            set;
            get;
        }
    }
}