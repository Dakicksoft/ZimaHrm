using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("AllowanceEmployee")]
    public class AllowanceEmployee : BaseEntity
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("Allowance")]
        public Guid AllowanceId { get; set; }
        public Allowance Allowance { get; set; }
    }
}
