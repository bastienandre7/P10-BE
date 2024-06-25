using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.IdentityModel.Tokens;
using PatientWebApi.Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PatientWebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _manager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration, RoleManager<IdentityRole> roleManager)
        {
            _manager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }

        public async Task<bool> Register(LoginUser user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.Email,
                Email = user.Email,
            };

            var result = await _manager.CreateAsync(identityUser, user.Password);

            return result.Succeeded;
        }

        public async Task<bool> Login(LoginUser user)
        {
            var identityUser = await _manager.FindByEmailAsync(user.Email);
            if(identityUser is null)
            {
                return false;
            }

            return await _manager.CheckPasswordAsync(identityUser, user.Password);
        }

        public string GenerateTokenString(LoginUser user)
        {
            IEnumerable<System.Security.Claims.Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,"Admin"),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var signingCred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var securityToken = new JwtSecurityToken(
                claims:claims,
                expires:DateTime.Now.AddMinutes(60),
                issuer:_configuration.GetSection("Jwt:Issuer").Value,
                audience: _configuration.GetSection("Jwt:Audience").Value,
                signingCredentials:signingCred);

            string tokenString = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return tokenString;
        }
        public async Task<bool> CreateRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
            {
                return true;
            }

            var identityRole = new IdentityRole(roleName);
            var result = await _roleManager.CreateAsync(identityRole);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleToUser(string email, string roleName)
        {
            var user = await _manager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            var result = await _manager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }
    }
}
