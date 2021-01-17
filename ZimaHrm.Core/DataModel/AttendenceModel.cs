using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    public class AttendenceModel : BaseModel
    {
        [Required]
        [DisplayName("Attendence Date")]
        public DateTime AttendenceDate { get; set; }
        [Required]
        public string Status { get; set; }
        public string Reason{ get; set; }

        [ForeignKey("EmployeeModel")]
        [Required]
        public Guid EmployeeId { get; set; }
        public EmployeeModel EmployeeModel { get; set; }
    }
}
