using ZimaHrm.Core.DataModel.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ZimaHrm.Core.DataModel
{
    [Table("Holiday")]
    public class HolidayModel : BaseModel
    {
        [Required]
        public string Month { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Day { get; set; }
        [Required]
        [DisplayName("Occasion")]
        public string Occesion { get; set; }
    }
}
