using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ZimaHrm.Data.Entity
{
    //public class User : BaseEntity
    //{
    //    [EmailAddress]
    //    [Required]
    //    public string Email { get; set; }
    //    [Required]
    //    public string Password { get; set; }
    //    [Required]
    //    public string Role { get; set; }
    //    public int? UserId { get; set; }
    //}

    public class User : IdentityUser<Guid>
    {
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 1=> All Good, 2=> Blocked
        /// </summary>
        public UserStatus Status { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string AvatarURL { get; set; }
        public string DateRegisteredUTC { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }

    public enum UserStatus : int
    {
        AllGood = 1,
        Blocked = 2,
    }
}
