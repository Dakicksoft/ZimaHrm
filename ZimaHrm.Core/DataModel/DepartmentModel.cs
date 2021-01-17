using ZimaHrm.Core.DataModel.Base;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Core.DataModel
{
    [Table("Department")]
    public class DepartmentModel : BaseModel
    {
        public DepartmentModel()
        {
            Designations = new Collection<DesignationModel>();
        }
        [Required]
        [Remote("DeptExists", "Common", HttpMethod = "POST", ErrorMessage = "Name Already exists")]
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<DesignationModel> Designations { get; set; }
    }
}
