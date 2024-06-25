using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PatientWebApi.Models;
using PatientWebApi.Services;

namespace PatientWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginUser user)
        {
            if (await _authService.Register(user))
            {
                return Ok("Done");
            }
            return BadRequest();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (await _authService.Login(user))
            {
                var tokenString = _authService.GenerateTokenString(user);
                var tokenResponse = new { Token = tokenString };
                return Ok(tokenResponse);
            }
            return BadRequest();
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name cannot be empty.");
            }

            var result = await _authService.CreateRole(roleName);
            if (result)
            {
                return Ok($"Role '{roleName}' created successfully.");
            }
            else
            {
                return StatusCode(500, "Error creating role.");
            }
        }
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] UserRoleAssignment model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.RoleName))
            {
                return BadRequest("Email and role name cannot be empty.");
            }

            var result = await _authService.AssignRoleToUser(model.Email, model.RoleName);
            if (result)
            {
                return Ok($"Role '{model.RoleName}' assigned to user '{model.Email}' successfully.");
            }
            else
            {
                return StatusCode(500, "Error assigning role to user.");
            }
        }
    }
}
