using Microsoft.AspNetCore.Mvc;
using ToDo_List.DTOs.Auth;
using ToDo_List.Services.Interfaces;

namespace ToDo_List.Controllers
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterReqDto dto)
        {
            var result = await _authService.Register(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReqDto dto)
        {
            var result = await _authService.Login(dto);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassReqDto dto)
        {
            var result = await _authService.GenerateResetToken(dto);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassReqDto dto)
        {
            var result = await _authService.ResetPassword(dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}