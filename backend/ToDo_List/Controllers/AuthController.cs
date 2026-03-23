using Microsoft.AspNetCore.Mvc;
using ToDo_List.DTOs.Auth;
using ToDo_List.Responses;
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

            return result.Success
                ? Ok(new ApiResponse<object> { Success = true, Message = result.Message })
                : BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginReqDto dto)
        {
            var result = await _authService.Login(dto);

            return result.Success
                ? Ok(new ApiResponse<string> { Success = true, Message = result.Message, Data = result.Token })
                : Unauthorized(new ApiResponse<object> { Success = false, Message = result.Message });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassReqDto dto)
        {
            var result = await _authService.GenerateResetToken(dto);

            return result.Success
                ? Ok(new ApiResponse<object>{ Success = true, Message = result.Message, Data = result.Token})
                : NotFound(new ApiResponse<object> { Success = false, Message = result.Message });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassReqDto dto)
        {
            var result = await _authService.ResetPassword(dto);

            return result.Success
                ? Ok(new ApiResponse<object> { Success = true, Message = result.Message })
                : BadRequest(new ApiResponse<object> { Success = false, Message = result.Message });
        }
    }
}