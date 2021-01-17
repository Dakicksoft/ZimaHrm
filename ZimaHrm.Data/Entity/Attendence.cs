using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Attendence")]
    public class Attendence : BaseEntity
    {
        [Required]
        public DateTime AttendenceDate { get; set; }
        [Required]
        public string Status { get; set; }
        public string Reason { get; set; }

        [ForeignKey("EmployeeModel")]
        [Required]
        public Guid EmployeeId { get; set; }
        public Employee EmployeeModel { get; set; }
    }
}
