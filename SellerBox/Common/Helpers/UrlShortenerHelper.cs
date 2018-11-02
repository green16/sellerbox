using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace SellerBox.Common.Helpers
{
    public class UrlShortenerHelper
    {
        public const string RoutePrefix = "sl";

        private static Random _random = new Random();
        private static readonly int[] guidByteOrder = { 15, 14, 13, 12, 11, 10, 9, 8, 6, 7, 4, 5, 0, 1, 2, 3 };

        private const string Alphabet = "23456789bcdfghjkmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ";

        private static readonly int Base = Alphabet.Length;

        public static BigInteger Convert(Guid guid)
        {
            var guidBytes = string.Format("0{0:N}", guid);
            return BigInteger.Parse(guidBytes, NumberStyles.HexNumber);
        }

        public static Guid Convert(BigInteger bigInteger)
        {
            var bigIntegerBytes = bigInteger.ToByteArray();
            var resultBytes = new byte[16];
            for (int i = 0; i < guidByteOrder.Length; i++)
            {
                int index = guidByteOrder[i];
                resultBytes[index] = bigIntegerBytes[i];
            }
            return new Guid(resultBytes);
        }

        public static string GetRandomUrlKey(int length)
        {
            var sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
                sb.Append(Alphabet.ElementAt(_random.Next(Base)));

            return sb.ToString();
        }

        public static string Encode(long num)
        {
            var sb = new StringBuilder();
            while (num > 0)
            {
                int elementIndex = (int)(num % Base);
                sb.Insert(0, Alphabet.ElementAt(elementIndex));
                num = num / Base;
            }
            return sb.ToString();
        }

        public static string Encode(Guid guid)
        {
            BigInteger bigInteger = Convert(guid);
            return Encode(bigInteger);
        }

        public static string Encode(BigInteger num)
        {
            var sb = new StringBuilder();
            while (num != 0)
            {
                int elementIndex = Math.Abs((int)(num % Base));
                sb.Insert(0, Alphabet.ElementAt(elementIndex));
                num = num / Base;
            }
            return sb.ToString();
        }

        public static BigInteger Decode(string str)
        {
            BigInteger num = new BigInteger(0);
            for (var i = 0; i < str.Length; i++)
                num = num * Base + Alphabet.IndexOf(str.ElementAt(i));

            return num;
        }
    }
}
