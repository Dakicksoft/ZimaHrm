using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    public class LeaveApplication : BaseEntity
    {
        [Required]
        public string Reason { get; set; }
        [Required]
        public string Status { get; set; }

        public DateTime LeaveDate { get; set; }


        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("LeaveType")]
        public Guid LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
    }
}
