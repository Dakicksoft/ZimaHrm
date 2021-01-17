using ZimaHrm.Core.DataModel.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Core.DataModel
{
    public class PaySlipModel : BaseModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [DisplayName("Department")]
        public Guid DepartmentId { get; set; }

        [Required]
        public string Month { get; set; }
        [Required]
        [DisplayName("Payment Date")]
        public DateTime PaymentDate { get; set; }
        public string Description { get; set; }
    }
}
