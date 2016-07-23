using AccountingTransactionService.Enums;

namespace AccountingTransactionService.DbEntities
{
    public class AccountSearchRule
    {
        public AccountSearchRule()
        {
        }

        public int Id
        {
            set;
            get;
        }

        public int AccountSearchTemplate
        {
            set;
            get;
        }


        public int AccountBindingTemplate
        {
            set;
            get;
        }

        public string AccountBindingTemplateName
        {
            set;
            get;
        }

        public EntityType EntityType1
        {
            set;
            get;
        }

        public EntityType EntityType2
        {
            set;
            get;
        }

        public int Priority
        {
            set;
            get;
        }
    }
}