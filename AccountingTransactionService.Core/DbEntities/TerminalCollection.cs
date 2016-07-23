using AccountingTransactionService.Configuration;
using AccountingTransactionService.Entities;
using AccountingTransactionService.Enums;
using AccountingTransactionService.Interfaces;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.DbEntities
{
    public class TerminalCollection : IAccountSearch
    {
        private long defaultEntityId;
        private int defaultEntityData;

        public TerminalCollection()
        {
            defaultEntityId = ConfigurationProvider.DefaultEntityId;
            defaultEntityData = ConfigurationProvider.DefaultData;
        }

        public int Id
        {
            set;
            get;
        }

        public string Name
        {
            set;
            get;
        }

        public string ExternalCode
        {
            set;
            get;
        }

        public int PayOfficeId
        {
            set;
            get;
        }

        public string PayOfficeName
        {
            set;
            get;
        }

        public int SubdealerId
        {
            set;
            get;
        }

        public string SubdealerName
        {
            set;
            get;
        }

        public Bank Subdealer
        {
            set;
            get;
        }

        public int AccountiongTransactionSheme
        {
            set;
            get;
        }

        public decimal Amount
        {
            set;
            get;
        }

        public EntityType ExcludeRecipientType
        {
            get
            {
                return EntityType.Gateway;
            }
        }

        public AccountFilter GetAccountFilter(int accBindingTemplateId, EntityType entityType1, EntityType entityType2, BankRequisites target, int eventPointId, long eventData)
        {
            AccountFilter result = new AccountFilter(accBindingTemplateId, defaultEntityId, string.Empty, defaultEntityId, string.Empty, defaultEntityData);

            result.EntityId1 = Id;
            result.EntityName1 = Name;

            return result;
        }

        public Bank GetBank(EntityType entityType1, EntityType entityType2)
        {
            return Subdealer;
        }

        public Bank GetBank(BankRequisites target)
        {
            return Subdealer;
        }

        public string GetAttributesErrorMessage(EntityType entityType1, EntityType entityType2)
        {
            string result = string.Empty;

            if (entityType1 == EntityType.Point && entityType2 == EntityType.None)
            {
                result = $"Не заданы параметры (Название, БИК, Кор.счет) для Филиала={SubdealerName}";
            }

            return result;
        }

        public string SubstituteVariableValues(string exp)
        {
            return string.Empty;
        }
    }
}