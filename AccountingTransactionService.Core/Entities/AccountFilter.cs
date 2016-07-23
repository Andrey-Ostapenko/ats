namespace AccountingTransactionService.Entities
{  
    public struct AccountFilter
    {
        private int accountBinddingTemplateId;
        private long entityId1;
        private string entityName1;
        private long entityId2;
        private string entityName2;
        private long data;

        public AccountFilter(int accountBinddingTemplateId, long entityId1, string entityName1, long entityId2, string entityName2, long data)
        {
            this.accountBinddingTemplateId = accountBinddingTemplateId;
            this.entityId1 = entityId1;
            this.entityName1 = entityName1;
            this.entityId2 = entityId2;
            this.entityName2 = entityName2;
            this.data = data;
        }

        public int AccountBinddingTemplateId
        {
            set { accountBinddingTemplateId = value; }
            get { return accountBinddingTemplateId; }
        }

        public long EntityId1
        {
            set { entityId1 = value; }
            get { return entityId1; }
        }

        public string EntityName1
        {
            set { entityName1 = value; }
            get { return entityName1; }
        }

        public long EntityId2
        {
            set { entityId2 = value; }
            get { return entityId2; }
        }

        public string EntityName2
        {
            set { entityName2 = value; }
            get { return entityName2; }
        }

        public long Data
        {
            set { data = value; }
            get { return data; }
        }
    }
}
