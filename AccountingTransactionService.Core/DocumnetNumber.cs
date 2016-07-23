using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AccountingTransactionService
{
    public class DocumentNumberSettings
    {
        public long Number
        {
            get;
            set;
        }

        public long Min
        {
            set;
            get;
        }

        public long Max
        {
            set;
            get;
        }
    }

    class DocumentNumber
    {
        public DocumentNumber(string filePath)
        {
            FilePath = filePath;
        }

        private string FilePath
        {
            set;
            get;
        }

        private object getEntity(string requisites, Type type)
        {
            object result;
            XmlSerializer xmlSerializer = new XmlSerializer(type);

            using (StringReader sr = new StringReader(requisites))
            {
                result = xmlSerializer.Deserialize(sr);
            }

            return result;
        }

        private string getString(object o, Type type)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(type);
            StringBuilder sb = new StringBuilder();

            using (StringWriter sw = new StringWriter(sb))
            {
                xmlSerializer.Serialize(sw, o);
            }

            return sb.ToString();
        }

        private long Number
        {
            set;
            get;
        }

        public void Read()
        {
            string fileText = File.ReadAllText(FilePath);

            DocumentNumberSettings counter = (DocumentNumberSettings)getEntity(fileText, typeof(DocumentNumberSettings));

            Number = counter.Number;
            Min = counter.Min;
            Max = counter.Max;
        }

        public void Save()
        {
            DocumentNumberSettings counter = new DocumentNumberSettings();
            counter.Number = Number;
            counter.Min = Min;
            counter.Max = Max;
            string fileContent = getString(counter, typeof(DocumentNumberSettings));
            File.WriteAllText(FilePath, fileContent);
        }

        public long Min
        {
            private set;
            get;
        }

        public long Max
        {
            private set;
            get;
        }

        public long Next
        {
            get
            {
                Number++;

                if (Number > Max)
                {
                    Number = Min;
                }

                return Number;
            }
        }
    }
}
