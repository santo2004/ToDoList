using System.ComponentModel.DataAnnotations;

namespace ToDo_List.DTOs.Auth
{
    public class LoginReqDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}