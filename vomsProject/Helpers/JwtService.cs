using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using vomsProject.Data;

/*
 This class is to help keep jwt token configuration in sync for valdiation and creation.
 */
namespace vomsProject.Helpers
{
    public class JwtService
    {
        public static readonly string Issuer = "http://localhost:5001/";
        public JwtService(IConfiguration configurtion)
        {
            SymmetricSecurityKey = new SymmetricSecurityKey(Convert.FromBase64String(configurtion.GetValue<string>("TokenSigningKey")));
            TokenValidationParamters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidAudience = Issuer,
                IssuerSigningKey = SymmetricSecurityKey
            };
        }

        private SymmetricSecurityKey SymmetricSecurityKey;
        public TokenValidationParameters TokenValidationParamters;
        
        public JwtSecurityToken CreateOneTimeToken(User user)
        {
            var claims = new Claim[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            return new JwtSecurityToken(
                Issuer,
                Issuer,
                claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: new SigningCredentials(SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );
        }
        
    }
}
