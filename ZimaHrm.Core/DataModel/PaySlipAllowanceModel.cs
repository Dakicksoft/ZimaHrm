using ZimaHrm.Core.DataModel.Base;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Core.DataModel
{
    public class PaySlipAllowanceModel : BaseModel
    {
        [Required]
        [DisplayName("Allowance Name")]
        public string AllowanceName { get; set; }
        [Required]
        [DisplayName("Allowance Type")]
        public string AllowanceType { get; set; }

        [Required]
        [DisplayName("Value")]
        public bool IsValue { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public double Amount { get; set; }
        

        [ForeignKey("EmployeePaySlipModel")]
        public int EmployeePaySlipId { get; set; }
        public EmployeePaySlipModel EmployeePaySlipModel { get; set; }
    }
}
