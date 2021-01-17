using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    public class LeaveEmployeeModel : BaseModel
    {
        [ForeignKey("Employee")]
        [DisplayName("Employee")]
        public int EmployeeId { get; set; }
        public EmployeeModel Employee { get; set; }

        [ForeignKey("LeaveGroup")]
        [DisplayName("Leave Group")]
        public int LeaveGroupId { get; set; }
        public LeaveGroupModel LeaveGroup { get; set; }
      
    }
}
