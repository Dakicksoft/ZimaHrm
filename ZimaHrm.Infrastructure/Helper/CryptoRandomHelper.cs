using ZimaHrm.Core.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Helper
{

    public class CryptoRandomHelper
    {
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public static byte[] CreateRandomBytes(int length)
        {
            var bytes = new byte[length];
            _rng.GetBytes(bytes);

            return bytes;
        }

        public static string CreateRandomKey(int length)
        {
            var bytes = new byte[length];
            _rng.GetBytes(bytes);

            return Convert.ToBase64String(CreateRandomBytes(length));
        }

        public static string CreateUniqueKey(int length = 8)
        {
            return CreateRandomBytes(length).ToHexString();
        }

        public static string CreateSeriesNumber(string prefix = "MSK")
        {
            return $"{prefix}{DateTime.Now.ToString("yyyyMMddHHmmss")}{CreateUniqueKey()}";
        }
    }
}
