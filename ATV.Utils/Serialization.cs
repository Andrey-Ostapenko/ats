using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ATV.Utils
{
    public static class SimpleStringXmlSerializer
    {
        public static T Deserialize<T>(string xml) where T : class
        {
            T result;
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(xml))
            {
                result = xmlSerializer.Deserialize(sr) as T;
            }

            return result;
        }

        public static string Serialize<T>(T o) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb))
            {
                xmlSerializer.Serialize(sw, o);
            }

            return sb.ToString();
        }
    }
}
