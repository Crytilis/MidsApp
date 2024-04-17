using System.Text;
using System;
using System.Numerics;

namespace MidsApp.Utils
{
    /// <summary>
    /// Provides methods for transforming data between byte arrays and a Base62 encoding.
    /// </summary>
    public static class Transform
    {
        private const string BaseChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Encodes a byte array to a Base62 string.
        /// </summary>
        /// <param name="byteArray">The byte array to encode.</param>
        /// <returns>The encoded Base62 string.</returns>
        /// <remarks>
        /// The BigInteger constructor expects a byte array in little-endian order, 
        /// but BigInteger's ToByteArray method returns in little-endian as well. 
        /// When encoding, the array is reversed to match the expected order.
        /// </remarks>
        public static string ToBase62(byte[] byteArray)
        {
            var bigInt = new BigInteger(byteArray, isBigEndian: true);
            return Base62FromBigInt(bigInt);
        }

        /// <summary>
        /// Decodes a Base62 encoded string back to a byte array.
        /// </summary>
        /// <param name="encodedString">The Base62 encoded string to decode.</param>
        /// <returns>The decoded byte array.</returns>
        /// <remarks>
        /// When decoding, the BigInteger's byte array is converted back considering the little-endian format.
        /// </remarks>
        public static byte[] FromBase62(string encodedString)
        {
            var value = Base62ToBigInt(encodedString);
            var byteArray = value.ToByteArray(isBigEndian: true);
            return byteArray;
        }

        /// <summary>
        /// Converts a BigInteger to a Base62 string.
        /// </summary>
        /// <param name="value">The BigInteger value to convert.</param>
        /// <returns>The converted Base62 string.</returns>
        private static string Base62FromBigInt(BigInteger value)
        {
            if (value == 0) return "0"; // Ensure zero is correctly represented.
            var result = new StringBuilder();
            while (value > 0)
            {
                result.Insert(0, BaseChars[(int)(value % 62)]);
                value /= 62;
            }

            return result.ToString();
        }

        /// <summary>
        /// Converts a Base62 string to a BigInteger.
        /// </summary>
        /// <param name="base62">The Base62 string to convert.</param>
        /// <returns>The BigInteger representation of the Base62 string.</returns>
        private static BigInteger Base62ToBigInt(string base62)
        {
            BigInteger result = 0;
            for (var i = 0; i < base62.Length; i++)
            {
                var digit = BaseChars.IndexOf(base62[i]);
                if (digit == -1)
                    throw new ArgumentException("Invalid character found.", nameof(base62));

                result += digit * BigInteger.Pow(62, base62.Length - i - 1);
            }

            return result;
        }
    }
}
