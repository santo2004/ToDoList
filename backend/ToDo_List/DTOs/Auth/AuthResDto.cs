namespace ToDo_List.DTOs.Auth
{
    public class AuthResDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? Token { get; set; }   
        public object? Data { get; set; }    
    }
}