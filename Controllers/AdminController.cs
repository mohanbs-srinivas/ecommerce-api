using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ecommerce_api.Models;
using ecommerce_api.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EcommerceContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            EcommerceContext context,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpGet("pending-staff-registrations")]
        public async Task<ActionResult<IEnumerable<object>>> GetPendingStaffRegistrations()
        {
            try
            {
                var pendingStaff = await _context.Staff
                    .Where(s => !s.IsApproved)
                    .Select(s => new
                    {
                        s.Id,
                        s.FirstName,
                        s.LastName,
                        s.Email,
                        s.EmployeeId,
                        s.Department,
                        s.CreatedAt,
                        s.IsEmailVerified
                    })
                    .ToListAsync();

                return Ok(pendingStaff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending staff registrations");
                return StatusCode(500, new { error = "An error occurred while retrieving pending registrations" });
            }
        }

        [HttpPost("approve-staff/{userId}")]
        public async Task<IActionResult> ApproveStaffRegistration(string userId, [FromQuery] string? approvedBy = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "User ID is required" });
            }

            try
            {
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == userId);
                if (staff == null)
                {
                    return NotFound(new { error = "Staff member not found" });
                }

                if (staff.IsApproved)
                {
                    return BadRequest(new { error = "Staff member is already approved" });
                }

                staff.IsApproved = true;
                staff.ApprovedAt = DateTime.UtcNow;
                staff.ApprovedBy = approvedBy ?? "admin";

                await _context.SaveChangesAsync();

                _logger.LogInformation("Staff member approved: {UserId} by {ApprovedBy}", userId, approvedBy);

                return Ok(new 
                { 
                    success = true, 
                    message = "Staff registration approved successfully",
                    userId = userId,
                    approvedAt = staff.ApprovedAt,
                    approvedBy = staff.ApprovedBy
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving staff registration for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while approving the registration" });
            }
        }

        [HttpPost("reject-staff/{userId}")]
        public async Task<IActionResult> RejectStaffRegistration(string userId, [FromBody] RejectStaffRequest request)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { error = "User ID is required" });
            }

            try
            {
                var staff = await _context.Staff.FirstOrDefaultAsync(s => s.Id == userId);
                if (staff == null)
                {
                    return NotFound(new { error = "Staff member not found" });
                }

                if (staff.IsApproved)
                {
                    return BadRequest(new { error = "Cannot reject an already approved staff member" });
                }

                // Delete the user account
                var result = await _userManager.DeleteAsync(staff);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new { error = "Failed to delete user account", errors = result.Errors.Select(e => e.Description) });
                }

                _logger.LogInformation("Staff registration rejected and deleted: {UserId} by {RejectedBy}. Reason: {Reason}", 
                    userId, request.RejectedBy ?? "admin", request.Reason ?? "No reason provided");

                return Ok(new 
                { 
                    success = true, 
                    message = "Staff registration rejected and account deleted",
                    userId = userId,
                    reason = request.Reason
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting staff registration for user {UserId}", userId);
                return StatusCode(500, new { error = "An error occurred while rejecting the registration" });
            }
        }

        [HttpGet("staff-statistics")]
        public async Task<ActionResult<object>> GetStaffStatistics()
        {
            try
            {
                var stats = new
                {
                    totalStaff = await _context.Staff.CountAsync(),
                    approvedStaff = await _context.Staff.CountAsync(s => s.IsApproved),
                    pendingStaff = await _context.Staff.CountAsync(s => !s.IsApproved),
                    emailVerifiedStaff = await _context.Staff.CountAsync(s => s.IsEmailVerified),
                    totalMembers = await _context.Members.CountAsync(),
                    activeMembers = await _context.Members.CountAsync(m => m.IsActive)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff statistics");
                return StatusCode(500, new { error = "An error occurred while retrieving statistics" });
            }
        }
    }

    public class RejectStaffRequest
    {
        public string? Reason { get; set; }
        public string? RejectedBy { get; set; }
    }
}