using System.Xml.Serialization;

namespace AccountingTransactionService.XmlEntities
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
}