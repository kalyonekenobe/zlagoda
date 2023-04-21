namespace Zlagoda.Models
{
    public class AuthViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public dynamic Errors { get; set; } = new List<string>();   
    }
}
