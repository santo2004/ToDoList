using Microsoft.EntityFrameworkCore;
using ToDo_List.Auth;
using ToDo_List.Data;
using ToDo_List.DTOs.Auth;
using ToDo_List.Services.Interfaces;

namespace ToDo_List.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;

        public AuthService( AppDbContext context, IPasswordHasher passwordHasher, IJwtService jwtService)
        { 
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResDto> Register(RegisterReqDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return new AuthResDto { Success = false, Message = "Passwords do not match" };

            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return new AuthResDto { Success = false, Message = "User already exists" };

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new AuthResDto
                {
                    Success = false,
                    Message = ex.InnerException?.Message ?? "Database error"
                };
            }

            return new AuthResDto
            {
                Success = true,
                Message = "User registered successfully"
            };
        }

        public async Task<AuthResDto> Login(LoginReqDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !_passwordHasher.Verify(dto.Password, user.PasswordHash))
                return new AuthResDto { Success = false, Message = "Invalid credentials" };

            return new AuthResDto
            {
                Success = true,
                Message = "Login successful",
                Token = _jwtService.GenerateToken(user)
            };
        }

        public async Task<AuthResDto> GenerateResetToken(ForgotPassReqDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return new AuthResDto { Success = false, Message = "User not found" };

            var token = Guid.NewGuid().ToString();

            user.ResetToken = token;
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            return new AuthResDto
            {
                Success = true,
                Message = "Reset token generated",
                Token = token   
            };
        }

        public async Task<AuthResDto> ResetPassword(ResetPassReqDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return new AuthResDto { Success = false, Message = "User not found" };

            if (user.ResetToken != dto.Token || user.ResetTokenExpiry < DateTime.UtcNow)
                return new AuthResDto { Success = false, Message = "Invalid or expired token" };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;

            await _context.SaveChangesAsync();

            return new AuthResDto
            {
                Success = true,
                Message = "Password reset successful"
            };
        }
    }
}