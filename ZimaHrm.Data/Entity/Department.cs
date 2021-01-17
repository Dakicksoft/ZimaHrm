using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Department")]
    public class Department : BaseEntity
    {
        public Department()
        {
            Designations = new Collection<Designation>();
        }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Designation> Designations { get; set; }
    }
}
