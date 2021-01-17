using ZimaHrm.Core.DataModel.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Core.DataModel
{
    public class LeaveApplicationModel : BaseModel
    {
        [Required]
        public string Reason { get; set; }
        [Required]
        public string Status { get; set; }

        [DisplayName("Leave Date")]
        public DateTime LeaveDate { get; set; }


        [ForeignKey("EmployeeModel")]
        public Guid EmployeeId { get; set; }
        public EmployeeModel EmployeeModel { get; set; }

        [ForeignKey("LeaveType")]
        [DisplayName("Leave Type")]
        public Guid LeaveTypeId { get; set; }
        public LeaveTypeModel LeaveType { get; set; }
    }
}
