using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("EmployeePaySlip")]
    public class EmployeePaySlip : BaseEntity
    {
        public EmployeePaySlip()
        {
            PaySlipAllowances = new Collection<PaySlipAllowance>();
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
        public Employee Employee { get; set; }

        [ForeignKey("PaySlip")]
        public Guid PaySlipId { get; set; }
        public PaySlip PaySlip { get; set; }

        public ICollection<PaySlipAllowance> PaySlipAllowances { get; set; }
    }
}
