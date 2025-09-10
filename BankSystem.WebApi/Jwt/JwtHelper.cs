using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankSystem.WebApi.Jwt
{
    public static class JwtHelper
    {
        public static string GenerateJwt(JwtDto jwtDto)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtDto.SecretKey));

            var credientals = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Id", jwtDto.Id.ToString()),
                new Claim("Email", jwtDto.Email),
                new Claim("FirstName", jwtDto.FirstName),
                new Claim("LastName", jwtDto.LastName),
                new Claim("UserType", jwtDto.UserType.ToString()),

                new Claim(ClaimTypes.Role, jwtDto.UserType.ToString())
            };

            var expireTime = DateTime.Now.AddMinutes(jwtDto.ExpireMinutes);

            var tokenDescriptor = new JwtSecurityToken(jwtDto.Issuer, jwtDto.Audience, claims, null, expireTime, credientals);

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;
        }
    }
}

