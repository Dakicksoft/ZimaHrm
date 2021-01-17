using System;

namespace ZimaHrm.Core.Infrastructure.File
{
    public class FileBinary
    {
        public FileBinary(Guid id, string name, byte[] bytes, long length, string contentType) : this(id, name, bytes, length)
        {
            ContentType = contentType;
        }

        public FileBinary(Guid id, string name, byte[] bytes, long length)
        {
            Id = id;
            Name = name;
            Bytes = bytes;
            Length = length;
        }

        public byte[] Bytes { get; set; }

        public string ContentType { get; set; }

        public Guid Id { get; set; }

        public long Length { get; set; }

        public string Name { get; set; }
    }
}
