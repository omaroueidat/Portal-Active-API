using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).{8,}$", ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, and at least one symbol.")]

        public string Password { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
