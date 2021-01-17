using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    public class PaySlipAllowance : BaseEntity
    {
        [Required]
        public string AllowanceName { get; set; }
        [Required]
        public string AllowanceType { get; set; }

        [Required]
        public bool IsValue { get; set; }
        [Required]
        public double Value { get; set; }
        [Required]
        public double Amount { get; set; }


        [ForeignKey("EmployeePaySlip")]
        public Guid EmployeePaySlipId { get; set; }
        public EmployeePaySlip EmployeePaySlip { get; set; }
    }
}
