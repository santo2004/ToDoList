using ToDo_List.DTOs.Auth;

namespace ToDo_List.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResDto> Register(RegisterReqDto dto);

        Task<AuthResDto> Login(LoginReqDto dto);

        Task<AuthResDto> GenerateResetToken(ForgotPassReqDto dto);

        Task<AuthResDto> ResetPassword(ResetPassReqDto dto);
    }
}