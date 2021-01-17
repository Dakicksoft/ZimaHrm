using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZimaHrm.Data.Entity
{
    [Table("Allowance")]
    public class Allowance : BaseEntity
    {
        [Required]
        public string AllowanceType { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public bool IsValue { get; set; }
        [Required]
        public double Value { get; set; }

        [NotMapped]
        public bool isCheck { get; set; }

    }
}
