using System.Collections.Generic;
using System.Security.Claims;

namespace ZimaHrm.Core.Infrastructure.Security
{
    public interface IJsonWebToken
    {
        Dictionary<string, object> Decode(string token);

        string Encode(IList<Claim> claims);
    }
}
