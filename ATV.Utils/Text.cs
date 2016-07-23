using System;
using System.Text;

namespace ATV.Utils
{
    public class TextEncoder
    {
        public TextEncoder(Encoding sourceEncoding, Encoding destEncoding)
        {
            if (SourceEncoding == null) throw new ArgumentNullException("Недопустимое значение аргумента", "sourceEncoding");
            if (DestEncoding == null) throw new ArgumentNullException("Недопустимое значение аргумента", "destEncoding");
            this.SourceEncoding = sourceEncoding;
            this.DestEncoding = destEncoding;
        }

        public Encoding SourceEncoding
        {
            private set;            
            get;
        }

        public Encoding DestEncoding
        {
            private set;
            get;
        }

        public string EncodeText(string text)
        {
            if (string.IsNullOrEmpty(text)) throw new ArgumentException("Недопустимое значение аргумента (строка не должна быть пустой или null)");

            if (SourceEncoding == null) throw new InvalidOperationException("sourceEncoding == null");

            if (DestEncoding == null) throw new InvalidOperationException("destEncoding == null");

            byte[] sourceBytes = SourceEncoding.GetBytes(text);
            byte[] destBytes = Encoding.Convert(SourceEncoding, DestEncoding, sourceBytes);
            return DestEncoding.GetString(destBytes);
        }
    }
}
