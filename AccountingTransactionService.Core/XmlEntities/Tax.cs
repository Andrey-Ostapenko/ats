using System.Xml.Serialization;

namespace AccountingTransactionService.XmlEntities
{
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
}