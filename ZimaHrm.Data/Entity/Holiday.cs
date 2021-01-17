using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Holiday")]
    public class Holiday : BaseEntity
    {
        [Required]
        public string Month { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Day { get; set; }
        [Required]
        public string Occesion { get; set; }
    }
}
