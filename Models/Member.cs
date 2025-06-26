using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class Member : ApplicationUser
    {
        [StringLength(20)]
        public string? MembershipNumber { get; set; }
        
        public DateTime? MembershipStartDate { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}