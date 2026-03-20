using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo_List.Data;
using ToDo_List.DTOs.Auth;
using ToDo_List.Services.Interfaces;

namespace ToDo_List.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

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

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return new AuthResDto { Success = false, Message = "Invalid credentials" };

            return new AuthResDto
            {
                Success = true,
                Message = "Login successful",
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<AuthResDto> GenerateResetToken(ForgotPassReqDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return new AuthResDto { Success = false, Message = "User not found" };

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            return new AuthResDto
            {
                Success = true,
                Message = "Reset token generated"
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

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}