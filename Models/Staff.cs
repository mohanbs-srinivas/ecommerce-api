using System.ComponentModel.DataAnnotations;

namespace ecommerce_api.Models
{
    public class Staff : ApplicationUser
    {
        [StringLength(20)]
        public string? EmployeeId { get; set; }
        
        [StringLength(50)]
        public string? Department { get; set; }
        
        public bool IsApproved { get; set; } = false;
        
        public DateTime? ApprovedAt { get; set; }
        
        public string? ApprovedBy { get; set; }
    }
}