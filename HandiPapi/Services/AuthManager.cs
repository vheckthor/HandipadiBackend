using HandiPapi.DataAccess;
using HandiPapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HandiPapi.Services
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<ApiUser> userManager;
        private readonly IConfiguration configuration;
        private ApiUser? user;

        public AuthManager(UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var token = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var expiration = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));
            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("validIssuer").Value,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);
            return token;

        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            };
            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("KEY");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        
        public async Task<bool> ValidateUser(LoginDto userDto)
        {
            user = await userManager.FindByNameAsync(userDto.Email);
            return (user != null && await userManager.CheckPasswordAsync(user, userDto.Password));
        }
    }
}
