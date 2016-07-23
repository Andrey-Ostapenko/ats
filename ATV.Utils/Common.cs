using System;
using System.Net;
using System.Text.RegularExpressions;

namespace ATV.Utils
{
    public static class HexConverter
    {
        public static string ToString(byte[] array)
        {
            if (array == null) throw new ArgumentNullException("Неправильный формат входных данных", "array");
            if (array.Length == 0) throw new ArgumentException("Неправильный формат входных данных", "array");

            return BitConverter.ToString(array).Replace("-", string.Empty);
        }

        public static byte[] ToByteArray(string hexString)
        {            
            if (string.IsNullOrEmpty(hexString) || hexString.Length % 2 != 0)
            {
                throw new ArgumentException("Неправильный формат входных данных", "hexString");
            }

            if (!Regex.IsMatch(hexString, "^[0-9a-fA-F]{2,}", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("Неправильный формат входных данных", "hexString");
            }

            byte[] result = new Byte[hexString.Length / 2];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return result;
        }
    }

    public static class IPAddressParser
    {
        public static IPAddress Parse(string strIpAddress)
        {
            if (strIpAddress == null)
            {
                throw new ArgumentNullException("Неправильный формат входных данных", "strIpAddress");
            }

            if (!Regex.IsMatch(strIpAddress, @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}\z"))
            {
                throw new ArgumentException("Неправильный формат входных данных", "strIpAddress");
            }

            MatchCollection matchCollection = Regex.Matches(strIpAddress, @"\d{1,3}");

            byte[] ip = new byte[4];
            int j = 0;

            foreach (Match match in matchCollection)
            {
                int value = int.Parse(match.Value);
                if (value > 255) throw new ArgumentException("Указанная строка не является IP адресом", "strIpAddress");
                ip[j] = (byte)value;
                j++;
            }

            return new IPAddress(ip);
        }
    }

    public static class ByteArrayXorer
    {
        public static byte[] XorData(byte[] arrayA, byte[] arrayB)
        {
            if (arrayA == null) throw new ArgumentNullException("Неправильный формат входных данных", "arrayA");
            if (arrayB == null) throw new ArgumentNullException("Неправильный формат входных данных", "arrayB");
            if (arrayA.Length != arrayB.Length) throw new ArgumentException("arrayA.Length != arrayB.Length");

            byte[] result = new byte[arrayA.Length];

            for (int i = 0; i < arrayA.Length; i++)
            {
                result[i] = (byte)(arrayA[i] ^ arrayB[i]);
            }

            return result;
        }
    }
}
