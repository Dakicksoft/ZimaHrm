using ZimaHrm.Core.DataModel.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace ZimaHrm.Core.DataModel
{
    public class UserModel : BaseModel 
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Role { get; set; }
        public Guid? UserId { get; set; }
    }
}
