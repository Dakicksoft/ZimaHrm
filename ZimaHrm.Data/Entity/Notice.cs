using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Data.Entity
{
    public class Notice : BaseEntity
    {
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
        
    }
}
