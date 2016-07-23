using System;
using AccountingTransactionService.Enums;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.Entities
{  
    class AccountingTransactionDetail
    {
        public AccountingTransactionDetail()
        {
            Id = -1;
        }

        public long Id
        {
            set;
            get;
        }

        public string AccTranNumber
        {
            set;
            get;
        }

        public int AccountingTransactionOrder
        {
            set;
            get;
        }

        public long PaymentId
        {
            set;
            get;
        }

        public string SessionNumber
        {
            set;
            get;
        }

        public string InitialSessionNumber
        {
            set;
            get;
        }

        public long GatewayId
        {
            set;
            get;
        }

        public long OperatorId
        {
            set;
            get;
        }

        public DocumentType DocumentType
        {
            set;
            get;
        }

        public AmountType AmountType
        {
            set;
            get;
        }

        public EntityType DebetEntityType1
        {
            set;
            get;
        }

        public long DebetEntityId1
        {
            set;
            get;
        }

        public EntityType DebetEntityType2
        {
            set;
            get;
        }

        public long DebetEntityId2
        {
            set;
            get;
        }

        public EntityType CreditEntityType1
        {
            set;
            get;
        }

        public long CreditEntityId1
        {
            set;
            get;
        }

        public EntityType CreditEntityType2
        {
            set;
            get;
        }

        public long CreditEntityId2
        {
            set;
            get;
        }

        public string DebetAccount
        {
            set;
            get;
        }

        public string DebetAccountName
        {
            set;
            get;
        }

        public Bank DebetBank
        {
            set;
            get;
        }

        public string CreditAccount
        {
            set;
            get;
        }

        public Bank CreditBank
        {
            set;
            get;
        }

        public string CreditAccountName
        {
            set;
            get;
        }

        public OrganizationPayer Payer
        {
            set;
            get;
        }

        public Organization Recipient
        {
            set;
            get;
        }


        public decimal Amount
        {
            set;
            get;
        }

        public string Description
        {
            set;
            get;
        }

        public string KBK
        {
            set;
            get;
        }

        public string OKATO
        {
            set;
            get;
        }

        public int? Symbol
        {
            set;
            get;
        }

        public string PointExternalCode
        {
            set;
            get;
        }

        public decimal AmountAll
        {
            set;
            get;
        }

        public decimal ClientCommission
        {
            set;
            get;
        }

        public decimal IntegratorCommission
        {
            set;
            get;
        }

        public decimal RewardAmount
        {
            set;
            get;
        }

        public DateTime PaymentDate
        {
            set;
            get;
        }

        public OtpOperationType OtpOperationType
        {
            set;
            get;
        }

        public string CommissionAccount
        {
            set;
            get;
        }

        public string VbUser
        {
            set;
            get;
        }
    }
}
