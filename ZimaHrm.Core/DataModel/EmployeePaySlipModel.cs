using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    public class EmployeePaySlipModel : BaseModel
    {
        public EmployeePaySlipModel()
        {
            PaySlipAllowances = new Collection<PaySlipAllowanceModel>();
        }
        [DisplayName("Basic Salary")]
        public double BasicSalary { get; set; }
        [DisplayName("Allowance Total")]
        public double AllowanceTotal { get; set; }
        [DisplayName("Deduction Total")]
        public double DeductionTotal { get; set; }
        [DisplayName("Net Salary")]
        public double NetSalary { get; set; }
        public string Status { get; set; }

        [ForeignKey("Employee")]
        public Guid EmployeeId { get; set; }
        public EmployeeModel Employee { get; set; }

        [ForeignKey("PaySlip")]
        public Guid PaySlipId { get; set; }
        public PaySlipModel PaySlip { get; set; }

        public ICollection<PaySlipAllowanceModel> PaySlipAllowances { get; set; }
    }
}
