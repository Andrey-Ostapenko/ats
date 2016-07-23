using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace ATV.Utils
{
    public interface IPinBlockFormater
    {
        byte[] GetPlainPinBlock(string cardNumber, string pin);
    }

    public class PinBlockFormatterISO0 : IPinBlockFormater
    {
        private const int minPinLength = 4;
        private const int maxPinLength = 8;
        private const int cardDataLength = 12;
        private const int cardBlockLength = 16;

        private int minCardNumberLength = 0;
        private int maxCardNumberLength = 0;

        public PinBlockFormatterISO0()
        {
            MinCardNumberLength = 16;
            MaxCardNumberLength = 16;
        }

        public PinBlockFormatterISO0(int minCardNumberLength, int maxCardNumberLength)
        {
            MinCardNumberLength = minCardNumberLength;
            MaxCardNumberLength = maxCardNumberLength;
        }

        public int MinCardNumberLength
        {
            set
            {
                if (value >= 0 && value <= 32 && (MinCardNumberLength < MaxCardNumberLength || MaxCardNumberLength == 0))
                {
                    minCardNumberLength = value;
                }
                else
                {
                    throw new ArgumentException("Минимальное значение номера карты должно быть больше 0");
                }
            }

            get
            {
                return minCardNumberLength;
            }
        }

        public int MaxCardNumberLength
        {
            set
            {
                if (value >= MinCardNumberLength && value <= 32) throw new ArgumentException("Минимальное значение номера карты должно быть больше 0");
                minCardNumberLength = value;
            }

            get
            {
                return minCardNumberLength;
            }
        }

        private byte[] getCardData(string cardNumber)
        {
            string cardData = cardNumber;

            if (cardNumber.Length > cardDataLength)
            {
                cardData = cardNumber.Substring(cardNumber.Length - (cardDataLength + 1), cardDataLength);
            }

            cardData = cardData.PadLeft(cardBlockLength, '0');

            return HexConverter.ToByteArray(cardData);
        }

        private byte[] getPinData(string pin)
        {
            if (pin.Length > maxPinLength || string.IsNullOrEmpty(pin)) throw new ArgumentException("Неправильное значение аргумента", "pin");

            string pinData = string.Format("{0}{1}", pin.Length.ToString().PadLeft(2, '0'), pin).PadRight(16, 'F');

            return HexConverter.ToByteArray(pinData);
        }

        public byte[] GetPlainPinBlock(string cardNumber, string pin)
        {
            if (string.IsNullOrEmpty(cardNumber) || !Regex.IsMatch(cardNumber, "^[0-9]{" + minCardNumberLength + "," + maxCardNumberLength + "}"))
            {
                throw new ArgumentException("Неправильный формат входных данных", "cardNumber");
            }

            if (pin.Length > maxPinLength || string.IsNullOrEmpty(pin) || !Regex.IsMatch(pin, "^[0-9]{" + minPinLength + "," + maxPinLength + "}"))
            {
                throw new ArgumentException("Неправильный формат входных данных", "pin");
            }

            return ByteArrayXorer.XorData(getCardData(cardNumber), getPinData(pin));
        }
    }

    public interface IEncryptor
    {
        byte[] Encrypt(byte[] data, byte[] key);
        byte[] Decrypt(byte[] data, byte[] key);
    }

    public class TripleDesEncryptor
    {
        private const int minKeySize = 16;
        private const int maxKeySize = 24;
        private SymmetricAlgorithm symAlgorithm;

        public TripleDesEncryptor()
        {
            symAlgorithm = new TripleDESCryptoServiceProvider();
            symAlgorithm.IV = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            symAlgorithm.Padding = PaddingMode.Zeros;
            symAlgorithm.Mode = CipherMode.ECB;
        }

        public TripleDesEncryptor(byte[] iv, PaddingMode paddingMode, CipherMode cipherMode)
        {
            symAlgorithm = new TripleDESCryptoServiceProvider();
            symAlgorithm.IV = iv;
            symAlgorithm.Padding = paddingMode;
            symAlgorithm.Mode = cipherMode;
        }

        private byte[] Encrypt(byte[] data, byte[] key)
        {
            if (data == null) throw new ArgumentNullException("Неправильное значение параметра", "data");
            if (data.Length == 0) throw new ArgumentException("Массив входных даннах имеет длинну 0 байт", "data");
            if (key == null) throw new ArgumentNullException("Неправильное значение параметра", "key");
            if (key.Length != minKeySize && key.Length != maxKeySize) throw new ArgumentException("Длинна ключа имеет неверное значение, длинна ключа может быть 16 или 24 байт", "key");

            symAlgorithm.Key = key;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, symAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    return ms.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] data, byte[] key)
        {
            if (data == null) throw new ArgumentNullException("Неправильное значение параметра", "data");
            if (data.Length == 0) throw new ArgumentNullException("Неправильное значение параметра", "data");
            if (key == null) throw new ArgumentNullException("Неправильное значение параметра", "key");
            if (key.Length != minKeySize && key.Length != maxKeySize) throw new ArgumentException("Неправильное значение параметра", "key");

            symAlgorithm.Key = key;

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, symAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] result = new byte[data.Length];
                    int lengthText = cs.Read(result, 0, data.Length);
                    return result;
                }
            }
        }
    }

    public class PinBlockBuilder
    {
        private IPinBlockFormater pinBlockFormatter;
        private IEncryptor encryptor;

        public PinBlockBuilder(IPinBlockFormater pinBlockFormatter, IEncryptor encryptor)
        {
            this.pinBlockFormatter = pinBlockFormatter;
            this.encryptor = encryptor;
        }

        public byte[] GetPinBlock(string pin, string cardNumber, byte[] masterKey, byte[] pinKey)
        {
            byte[] data = pinBlockFormatter.GetPlainPinBlock(cardNumber, pin);
            byte[] key = encryptor.Decrypt(pinKey, masterKey);
            return encryptor.Encrypt(data, key);
        }

        public string GetPinBlock(string pin, string cardNumber, string masterKey, string pinKey)
        {
            byte[] data = pinBlockFormatter.GetPlainPinBlock(cardNumber, pin);
            byte[] key = encryptor.Decrypt(HexConverter.ToByteArray(pinKey), HexConverter.ToByteArray(masterKey));
            return HexConverter.ToString(encryptor.Encrypt(data, key));
        }
    }
}
