using System;
using System.Security.Cryptography;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Security
{
    public class Hash : IHash
    {
        public string Create(string value)
        {
            var bytes = Create(Encoding.Unicode.GetBytes(value));

            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        public byte[] Create(byte[] value)
        {
            using (var algorithm = new SHA512Managed())
            {
                return algorithm.ComputeHash(value);
            }
        }
    }
}
