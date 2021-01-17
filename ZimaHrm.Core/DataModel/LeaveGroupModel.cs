using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    public class LeaveGroupModel : BaseModel
    {
        [DisplayName("Leave Group Name")]
        [Required]
        public string LeaveGroupName { get; set; }

        [DisplayName("Sick Leave")]
        [Required]
        public int SickLeave { get; set; }
        [DisplayName("Casual Leave")]
        [Required]
        public int CasualLeave { get; set; }
        [DisplayName("Half Day")]
        [Required]
        public int HalfDay { get; set; }
        [Required]
        public int Maternity { get; set; }
        [Required]
        public int UnPaid { get; set; }
        [Required]
        public int Others { get; set; }
    }
}
