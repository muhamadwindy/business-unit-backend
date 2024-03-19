using BusinessUnitApp.Models.Dtos;
using BusinessUnitApp.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusinessUnitApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("seedRoleAndUserAdmin")]
        public async Task<IActionResult> SeedRolesAnduserAdmin()
        {
            var seerRoles = await _authService.SeedRolesAndUserAdminAsync();
            return Ok(seerRoles);
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var registerResult = await _authService.RegisterAsync(registerDto);
             
            ResponseAPIDto result = new ResponseAPIDto()
            {
                status = registerResult.IsSucceed,
                message = registerResult.Message,
                data = registerDto
            };

            return Ok(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _authService.LoginAsync(loginDto);
            ResponseAPIDto result = new ResponseAPIDto()
            {
                status = loginResult.IsSucceed,
                message = loginResult.Message,
                data = loginDto
            };

            return Ok(result); 
        }

        [HttpGet]
        [Route("user")]
        public async Task<IActionResult> getUser()
        {
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token == null)
            {
                return Unauthorized();

            }
            var loginResult = await _authService.GetUser(HttpContext);

            return Ok(loginResult);

        }
    }
}