using ZimaHrm.Core.DataModel.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Core.DataModel
{
    [Table("Designation")]
    public class DesignationModel : BaseModel
    {
        [Required]
        [Remote("DesgExists", "Common", HttpMethod ="POST", ErrorMessage ="Name Already exists")]
        public string Name { get; set; }

        [ForeignKey("DepertmentModel")]
        [DisplayName("Depertment")]
        public Guid DepertmentId { get; set; }
        public DepartmentModel DepertmentModel { get; set; }
    }
}
