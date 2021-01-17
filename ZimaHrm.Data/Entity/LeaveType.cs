using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Data.Entity
{
    public class LeaveType : BaseEntity
    {
        [Required]
        public string LeaveTypeName { get; set; }
    }
}
