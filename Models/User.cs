using System;
using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
        
        public DateTime MembershipDate { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Member";
    }
}