using System.Xml.Serialization;

namespace AccountingTransactionService.XmlEntities
{
    [XmlRoot("params")]
    public class Organization
    {
        [XmlElement("organization_name")]
        public string Name { set; get; }

        [XmlElement("organization_inn")]
        public string INN { set; get; }

        [XmlElement("organization_kpp")]
        public string KPP { set; get; }

        [XmlElement("organization_account")]
        public string Account { set; get; }

        [XmlElement("organization_schema")]
        public string AccountingTransactionShema { set; get; }

        public bool IsValid
            =>
                !(string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(INN) || string.IsNullOrWhiteSpace(KPP) ||
                  string.IsNullOrWhiteSpace(Account));
    }
}