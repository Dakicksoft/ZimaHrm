using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    public static class ByteArrayExtensions
    {
        public static byte[] Decompress(this byte[] data, string encoding)
        {
            byte[] decompressedData = null;
            using (var outputStream = new MemoryStream())
            {
                using (var inputStream = new MemoryStream(data))
                {
                    if (encoding == "gzip")
                        using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
                        {
                            zip.CopyTo(outputStream);
                        }
                    else if (encoding == "deflate")
                        using (var zip = new DeflateStream(inputStream, CompressionMode.Decompress))
                        {
                            zip.CopyTo(outputStream);
                        }
                    else
                        throw new ArgumentException(string.Format("Unsupported encoding type {0}.", encoding), "encoding");
                }

                decompressedData = outputStream.ToArray();
            }

            return decompressedData;
        }

        public static async Task<byte[]> CompressAsync(this byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] compressesData;
            using (var outputStream = new MemoryStream())
            {
                using (var zip = new GZipStream(outputStream, CompressionMode.Compress, true))
                {
                    await zip.WriteAsync(data, 0, data.Length, cancellationToken).AnyContext();
                }

                await outputStream.FlushAsync(cancellationToken).AnyContext();
                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }

        /// <summary>
        /// Converts to base64 string.
        /// </summary>
        /// <param name="input">The input to convert.</param>
        /// <returns>The base64 string.</returns>
        public static string ToBase64String(this byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        /// <summary>
        /// Converts to base64 string and replace it's '+' to '-', '/' to '_' and '=' to '%3d'.
        /// </summary>
        /// <param name="input">The input to convert</param>
        /// <returns>The base64 string and replace it's '+' to '-', '/' to '_' and '=' to '%3d'</returns>
        public static string ToUrlSuitable(this byte[] input)
        {
            return input.ToBase64String().Replace("+", "-").Replace("/", "_").Replace("=", "%3d");
        }

        /// <summary>
        /// Converts byte array to hex string.
        /// </summary>
        /// <param name="bytes">The bytes to convert.</param>
        /// <returns>The hex string.</returns>
        public static string ToHexString(this byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}
