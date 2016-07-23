using System;
using AccountingTransactionService.XmlEntities;

namespace AccountingTransactionService.DbEntities
{
    public class Payment
    {
        public Payment() { }

        public override string ToString()
        {
            return
                $"Платёж Идентификатор={Id}, Сумма платежа={Amount}, Комиссия с клиента={ClientCommissionAmount}, Комиссия с получателя платежа={ContractCommissionAmount}, Банк={DealerName}, Филиал={SubdealerName}, Устройство={PointName}, Платёжный шлюз={GatewayName}, Провайдер={OperatorName}.";
        }

        public long Id
        {
            set;
            get;
        }

        public string SessionNumber
        {
            set;
            get;
        }

        public int DealerId
        {
            set;
            get;
        }

        public string DealerName
        {
            set;
            get;
        }

        public Bank Dealer
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

        public int PointId
        {
            set;
            get;
        }

        public string PointName
        {
            set;
            get;
        }

        public string PointExternalCode
        {
            set;
            get;
        }

        public int GatewayId
        {
            set;
            get;
        }

        public string GatewayName
        {
            set;
            get;
        }

        public Organization GatewayRecipient
        {
            set;
            get;
        }

        public Bank GatewayBank
        {
            set;
            get;
        }

        public long OperatorId
        {
            set;
            get;
        }

        public string OperatorName
        {
            set;
            get;
        }

        public int OperatorComposite
        {
            set;
            get;
        }

        public string PaymentTarget
        {
            set;
            get;
        }

        public OrganizationPayer OperatorPayer
        {
            set;
            get;
        }

        public Organization OperatorRecipient
        {
            set;
            get;
        }

        public Bank OperatorBank
        {
            set;
            get;
        }

        public decimal Amount
        {
            set;
            get;
        }

        public decimal AmountWithoutCommission
        {
            set;
            get;
        }

        public decimal ClientCommissionAmount
        {
            set;
            get;
        }

        public decimal ContractCommissionAmount
        {
            set;
            get;
        }

        public decimal IntegratorCommission
        {
            set;
            get;
        }

        public decimal AmountAll
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

        public string Parameters
        {
            set;
            get;
        }

        public DateTime Date
        {
            set;
            get;
        }

        public string PointAccount
        {
            set;
            get;
        }
    }
}