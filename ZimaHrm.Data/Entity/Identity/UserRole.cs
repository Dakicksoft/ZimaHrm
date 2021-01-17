using System;
using Microsoft.AspNetCore.Identity;

namespace ZimaHrm.Data.Entity
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public virtual User User { get; set; }

        public virtual Role Role { get; set; }
    }
}
