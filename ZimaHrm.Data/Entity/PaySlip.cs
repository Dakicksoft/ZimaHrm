using System;
using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Data.Entity
{
    public class PaySlip : BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public string Month { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }
    }
}
