using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ZimaHrm.Data.Entity
{
    public class Role : IdentityRole<Guid>
    {
        public bool IsSystemDefault { get; set; } = false;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public enum Roles
    {
        Admin,
        User
    }
}
