using System.Xml.Serialization;

namespace AccountingTransactionService.Configuration.Entities
{
    public class DbProviderSettings
    {
        [XmlAttribute("assembly")]
        public string Assembly
        {
            set;
            get;
        }

        [XmlAttribute("type")]
        public string Type
        {
            set;
            get;
        }
    }
}
