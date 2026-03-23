namespace ToDo_List.DTOs.Auth
{
    public class AuthResDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }   // JWT OR Reset Token
        public object? Data { get; set; }    // optional for future
    }
}