using System;
using Microsoft.AspNetCore.Identity;


namespace ZimaHrm.Data.Entity
{
    public class UserToken : IdentityUserToken<Guid>
    {
        //public DateTimeOffset ExpiresUtc { get; set; }
    }
}
