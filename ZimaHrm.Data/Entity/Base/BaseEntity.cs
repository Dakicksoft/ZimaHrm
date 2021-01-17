using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedUtc { get; set; }

        public string LastModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastModifiedUtc { get; set; }
        public bool IsDelete { get; set; }
    }
}
