using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zlagoda.Models;

namespace Zlagoda.Services
{
    public static class JwtTokenService
    {
        public static IConfiguration Configuration { get; set; } = null!;
        public static IHttpContextAccessor? HttpContextObject;

        public static string GenerateJwtToken(IEnumerable<Claim> claims, int tokenDurationInSeconds)
        {
            HttpContextObject!.HttpContext!.User = new ClaimsPrincipal(new ClaimsIdentity(claims));
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:secret"]));
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: Configuration["Jwt:issuer"],
                audience: Configuration["Jwt:audience"],
                notBefore: DateTime.Now,
                claims: claims,
                expires: DateTime.Now.AddSeconds(tokenDurationInSeconds),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public static IEnumerable<Claim> GetJwtTokenClaims(User user)
        {
            return new List<Claim>
            {
                new Claim("id_employee", user.id_employee.ToString()),
                new Claim("empl_surname", user.empl_surname),
                new Claim("empl_name", user.empl_name),
                new Claim("empl_patronymic", user.empl_patronymic ?? ""),
                new Claim("empl_role", user.empl_role),
                new Claim("salary", user.salary.ToString("0.00")),
                new Claim("date_of_birth", user.date_of_birth.ToString()),
                new Claim("date_of_start", user.date_of_start.ToString()),
                new Claim("city", user.city),
                new Claim("street", user.street),
                new Claim("zip_code", user.zip_code),
            };
        }

        public static string GenerateAccessToken(User user) => GenerateJwtToken(GetJwtTokenClaims(user), 600);

        public static string GenerateRefreshToken(User user) => GenerateJwtToken(GetJwtTokenClaims(user), 2592000);

        public static bool ValidateJwtToken(string token)
        {
            try
            {
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var jwtSecretKey = Encoding.UTF8.GetBytes(Configuration[$"Jwt:secret"]);
                var claimsPrincipal = jwtSecurityTokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSecretKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidAudience = Configuration[$"Jwt:audience"],
                    ValidIssuer = Configuration[$"Jwt:issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                }, out var _);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}