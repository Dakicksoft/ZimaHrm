using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Award")]
    public class Award : BaseEntity
    {
        [Required]
        public string AwardTitle { get; set; }

        [Required]
        public string Gift { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public string Month { get; set; }

        [ForeignKey("Employee")]
        [Required]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
