using System.Xml.Serialization;

namespace AccountingTransactionService.Types
{
    [XmlRoot("params")]
    public class Bank
    {
        public Bank()
        {
            Main = 0;
        }

        [XmlElement("bank_name")]
        public string Name
        {
            set;
            get;
        }

        [XmlElement("bank_bik")]
        public string BIK
        {
            set;
            get;
        }

        [XmlElement("bank_account")]
        public string CorrAccount
        {
            set;
            get;
        }

        [XmlElement("bank_main")]
        public int Main
        {
            set;
            get;
        }

        public bool IsValid
        {
            get
            {
                return !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(BIK));
            }
        }
    }

    [XmlRoot("params")]
    public class Organization
    {
        public Organization()
        {
        }

        [XmlElement("organization_name")]
        public string Name
        {
            set;
            get;
        }

        [XmlElement("organization_inn")]
        public string INN
        {
            set;
            get;
        }

        [XmlElement("organization_kpp")]
        public string KPP
        {
            set;
            get;
        }

        [XmlElement("organization_account")]
        public string Account
        {
            set;
            get;
        }

        [XmlElement("organization_schema")]
        public string AccountingTransactionShema
        {
            set;
            get;
        }

        public bool IsValid
        {
            get
            {
                return !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(INN) || string.IsNullOrWhiteSpace(KPP) || string.IsNullOrWhiteSpace(Account));
            }
        }
    }

    [XmlRoot("params")]
    public class OrganizationPayer
    {
        public OrganizationPayer()
        {
        }

        [XmlElement("payer_name")]
        public string Name
        {
            set;
            get;
        }

        [XmlElement("payer_inn")]
        public string INN
        {
            set;
            get;
        }

        [XmlElement("payer_kpp")]
        public string KPP
        {
            set;
            get;
        }

        /*
        [XmlElement("payer_account")]
        public string Account
        {
            set;
            get;
        }
        */

        public bool IsValid
        {
            get
            {
                return !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(INN) || string.IsNullOrWhiteSpace(KPP));
            }
        }
    }

    [XmlRoot("params")]
    public class Tax
    {
        public Tax()
        {
        }

        [XmlElement("kbk")]
        public string KBK
        {
            set;
            get;
        }

        [XmlElement("okato")]
        public string OKATO
        {
            set;
            get;
        }
    }

    [XmlRoot("params")]
    public class PaymentPurpose
    {
        [XmlElement("purpose")]
        public string Purpose
        {
            set;
            get;
        }
    }
}