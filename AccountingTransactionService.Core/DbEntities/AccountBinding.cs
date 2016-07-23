namespace AccountingTransactionService.DbEntities
{
    public class AccountBinding
    {
        public AccountBinding() { }

        public int AccountBindingTemplatateId
        {
            set;
            get;
        }

        public long EntityId1
        {
            set;
            get;
        }

        public long EntityId2
        {
            set;
            get;
        }

        public long Data
        {
            set;
            get;
        }

        public string AccountName
        {
            set;
            get;
        }

        public string Account
        {
            set;
            get;
        }

        public bool IsDynamic
        {
            set;
            get;
        }
    }
}