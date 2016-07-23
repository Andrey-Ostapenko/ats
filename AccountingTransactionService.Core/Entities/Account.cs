using AccountingTransactionService.Enums;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.Entities
{  
    class Account
    {
        public long AccountBinddingTemplateId
        {
            set;
            get;
        }

        public EntityType EntityType1
        {
            set;
            get;
        }

        public long EntityId1
        {
            set;
            get;
        }

        public string EntityName1
        {
            set;
            get;
        }

        public EntityType EntityType2
        {
            set;
            get;
        }

        public long EntityId2
        {
            set;
            get;
        }

        public string EntityName2
        {
            set;
            get;
        }

        public long Data
        {
            set;
            get;
        }

        public string Name
        {
            set;
            get;
        }

        public string Value
        {
            set;
            get;
        }

        public Bank Bank
        {
            set;
            get;
        }
    }
}
