using EmployeeOnboarding_DDMS.Aplication.DTOs.Auth;
using EmployeeOnboarding_DDMS.Aplication.Interfaces;
using EmployeeOnboarding_DDMS.Aplication.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeOnboarding_DDMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// User login
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<Response<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            if (!result.Succeeded)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// User registration
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<Response<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult<Response<bool>>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            if (userId == 0)
            {
                return Unauthorized(new Response<bool>("Invalid token."));
            }

            var result = await _authService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

