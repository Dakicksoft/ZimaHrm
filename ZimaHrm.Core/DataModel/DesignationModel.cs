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

        [ForeignKey("DepartmentModel")]
        [DisplayName("Department")]
        public Guid DepartmentId { get; set; }
        public DepartmentModel DepartmentModel { get; set; }
    }
}
