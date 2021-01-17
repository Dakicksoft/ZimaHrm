using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Designation")]
    public class Designation : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [ForeignKey("Department")]
        public Guid DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
