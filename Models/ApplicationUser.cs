using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsEmailVerified { get; set; } = false;
        
        public string UserType { get; set; } = "Member"; // Member, Staff
    }
}