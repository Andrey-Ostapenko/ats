using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using AccountingTransactionService.Configuration.Entities;

namespace AccountingTransactionService.Configuration
{
    public class AccountingTransactionServiceSettingsSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ModulesSettings));
            XmlReader reader = new XmlNodeReader(section);
            return serializer.Deserialize(reader);
        }
    }
}
