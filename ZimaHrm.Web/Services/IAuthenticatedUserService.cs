using System.Collections.Generic;
using System.Security.Claims;

namespace ZimaHrm.Web.Services
{
    public interface IAuthenticatedUserService
    {
        string UserId { get; }
        public string Username { get; }
        public string Name { get; }
        bool IsAuthenticated();
        IEnumerable<Claim> GetClaimsIdentity();
    }
}
