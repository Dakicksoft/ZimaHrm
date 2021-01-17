using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ZimaHrm.Core.Infrastructure.Security
{
    public class JsonWebToken : IJsonWebToken
    {
        public JsonWebToken(IJsonWebTokenSettings jsonWebTokenSettings)
        {
            JsonWebTokenSettings = jsonWebTokenSettings;

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JsonWebTokenSettings.Key));

            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);
        }

        private IJsonWebTokenSettings JsonWebTokenSettings { get; }

        private SigningCredentials SigningCredentials { get; }

        public Dictionary<string, object> Decode(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token).Payload;
        }

        public string Encode(IList<Claim> claims)
        {
            if (claims == null)
            {
                claims = new List<Claim>();
            }

            var jwtSecurityToken = new JwtSecurityToken
            (
                JsonWebTokenSettings.Issuer,
                JsonWebTokenSettings.Audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(JsonWebTokenSettings.Expires),
                SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
