using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class UserRegistrationDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Name must be between 1 and 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(255, ErrorMessage = "Email must be less than 255 characters")]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; } = string.Empty;
    }
    
    public class UserRegistrationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}