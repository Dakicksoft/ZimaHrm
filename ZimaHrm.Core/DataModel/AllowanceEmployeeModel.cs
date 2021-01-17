using ZimaHrm.Core.DataModel.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Core.DataModel
{
    public class AllowanceEmployeeModel : BaseModel
    {
        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public EmployeeModel Employee { get; set; }

        [ForeignKey("Allowance")]
        public Guid AllowanceId { get; set; }
        public AllowanceModel Allowance{ get; set; }
    }
}
