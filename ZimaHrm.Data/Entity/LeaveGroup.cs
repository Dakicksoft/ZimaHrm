using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Data.Entity
{
    public class LeaveGroup : BaseEntity
    {
        [Required]
        public string LeaveGroupName { get; set; }

        [Required]
        public int SickLeave { get; set; }
        [Required]
        public int CasualLeave { get; set; }
        [Required]
        public int HalfDay { get; set; }
        [Required]
        public int Maternity { get; set; }
        [Required]
        public int UnPaid { get; set; }
        [Required]
        public int Others { get; set; }
    }
}
