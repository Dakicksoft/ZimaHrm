using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    public class LeaveEmployee : BaseEntity
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [ForeignKey("LeaveGroup")]
        public Guid LeaveGroupId { get; set; }
        public LeaveGroup LeaveGroup { get; set; }
      
    }
}
