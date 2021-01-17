using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("AllowanceType")]
    public class AllowanceType : BaseEntity
    {
        [Required]
        public string AllowanceTypeName { get; set; }
    }
}
