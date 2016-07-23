using System.Xml.Serialization;

namespace AccountingTransactionService.Configuration.Entities
{
    [XmlRoot("modules")]
    public class ModulesSettings
    {
        [XmlElement("dbProvider")]
        public DbProviderSettings DbProviderSettings
        {
            set;
            get;
        }

        [XmlElement("gatewayProvider")]
        public GatewayProviderSettings GatewayProviderSettings
        {
            set;
            get;
        }
    }
}
