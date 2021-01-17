using System;
using System.IO;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        public static void CopyTo(this Stream fromStream, Stream toStream)
        {
            if (fromStream == null)
                throw new ArgumentNullException("fromStream");
            if (toStream == null)
                throw new ArgumentNullException("toStream");

            var bytes = new byte[8092];
            int dataRead;
            while ((dataRead = fromStream.Read(bytes, 0, bytes.Length)) > 0)
                toStream.Write(bytes, 0, dataRead);
        }

        public static byte[] ReadFully(this Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string ReadToString(this Stream stream)
        {
            // convert stream to string
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Returns the position to the beginning.
        /// </summary>
        public static void Reset(this MemoryStream ms)
        {
            ResetSeekableStream(ms);
        }

        /// <summary>
        /// Returns the position to the beginning.
        /// </summary>
        public static void Reset(this FileStream ms)
        {
            ResetSeekableStream(ms);
        }

        /// <summary>
        /// Returns the stream position to the beginning.
        /// This will throw an exception if the stream does not support seeking.
        /// </summary>
        private static void ResetSeekableStream(Stream s)
        {
            s.Seek(0, SeekOrigin.Begin);
        }
    }
}
