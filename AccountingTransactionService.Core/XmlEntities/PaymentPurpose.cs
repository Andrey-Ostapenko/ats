using System.Xml.Serialization;

namespace AccountingTransactionService.XmlEntities
{
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