using AccountingTransactionService.Enums;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.Entities
{  
    public class AccountingTransactionFields
    {
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

        public string PointExternalCode
        {
            set;
            get;
        }

        // учётные данные или ФИО кассира
        public string OperatorAccount
        {
            set;
            get;
        }

        public OrganizationPayer Payer
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

        public string CreditAccountName
        {
            set;
            get;
        }

        public Bank CreditBank
        {
            set;
            get;
        }

        public Organization Recipient
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
