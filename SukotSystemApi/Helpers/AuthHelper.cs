using Microsoft.IdentityModel.Tokens;
using SukotSystemCore.DTOs.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SukotSystemApi.Helpers
{
    public class AuthHelper
    {
        public static string CreateToken(LoginResultDTO login, IConfiguration configuration)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, login.UserId.ToString()),
                new Claim(ClaimTypes.Name, login.DisplayName),
                new Claim(ClaimTypes.HomePhone, login.Phone),
                new Claim(ClaimTypes.Role, login.Role)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Key")));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Jwt:Issuer"),
                audience: configuration.GetValue<string>("Jwt:Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: signinCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        }
    }
}
