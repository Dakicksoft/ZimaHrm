using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    public class AllowanceModel : BaseModel
    {
        [Required]
        [DisplayName("Allowance Type")]
        public string AllowanceType { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        [DisplayName("Value")]
        public bool IsValue { get; set; }
        [Required]
        public double Value { get; set; }

        [NotMapped]
        public bool isCheck { get; set; }

    }
}
