using System.Xml.Serialization;

namespace AccountingTransactionService.XmlEntities
{
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
        public bool IsValid => !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(INN) || string.IsNullOrWhiteSpace(KPP));
    }
}
 