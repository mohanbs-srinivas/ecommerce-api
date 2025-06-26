using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ecommerce_api.Models;
using ecommerce_api.Models.ViewModels;
using ecommerce_api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EcommerceContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            EcommerceContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        [HttpPost("register/member")]
        public async Task<ActionResult<RegistrationResponseViewModel>> RegisterMember(MemberRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponseViewModel
                {
                    Success = false,
                    Message = "Invalid input data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponseViewModel
                    {
                        Success = false,
                        Message = "Email already registered",
                        Errors = new List<string> { "A user with this email address already exists." }
                    });
                }

                // Create new member
                var member = new Member
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Phone = model.Phone,
                    UserType = "Member",
                    MembershipStartDate = DateTime.UtcNow,
                    MembershipNumber = GenerateMembershipNumber()
                };

                var result = await _userManager.CreateAsync(member, model.Password);

                if (result.Succeeded)
                {
                    // Generate email verification token
                    var emailToken = await GenerateEmailVerificationToken(member.Id);

                    _logger.LogInformation("Member registered successfully: {Email}", model.Email);

                    return Ok(new RegistrationResponseViewModel
                    {
                        Success = true,
                        Message = "Registration successful. Please check your email for verification instructions.",
                        UserId = member.Id,
                        RequiresEmailVerification = true
                    });
                }

                return BadRequest(new RegistrationResponseViewModel
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during member registration");
                return StatusCode(500, new RegistrationResponseViewModel
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        [HttpPost("register/staff")]
        public async Task<ActionResult<RegistrationResponseViewModel>> RegisterStaff(StaffRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponseViewModel
                {
                    Success = false,
                    Message = "Invalid input data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
                });
            }

            try
            {
                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponseViewModel
                    {
                        Success = false,
                        Message = "Email already registered",
                        Errors = new List<string> { "A user with this email address already exists." }
                    });
                }

                // Check if employee ID already exists
                var existingStaff = await _context.Staff.FirstOrDefaultAsync(s => s.EmployeeId == model.EmployeeId);
                if (existingStaff != null)
                {
                    return BadRequest(new RegistrationResponseViewModel
                    {
                        Success = false,
                        Message = "Employee ID already exists",
                        Errors = new List<string> { "A staff member with this employee ID already exists." }
                    });
                }

                // Create new staff member
                var staff = new Staff
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    Phone = model.Phone,
                    UserType = "Staff",
                    EmployeeId = model.EmployeeId,
                    Department = model.Department,
                    IsApproved = false // Requires admin approval
                };

                var result = await _userManager.CreateAsync(staff, model.Password);

                if (result.Succeeded)
                {
                    // Generate email verification token
                    var emailToken = await GenerateEmailVerificationToken(staff.Id);

                    _logger.LogInformation("Staff member registered successfully: {Email}", model.Email);

                    return Ok(new RegistrationResponseViewModel
                    {
                        Success = true,
                        Message = "Registration successful. Your account requires admin approval and email verification.",
                        UserId = staff.Id,
                        RequiresEmailVerification = true,
                        RequiresApproval = true
                    });
                }

                return BadRequest(new RegistrationResponseViewModel
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during staff registration");
                return StatusCode(500, new RegistrationResponseViewModel
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Errors = new List<string> { "Please try again later." }
                });
            }
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new { success = false, message = "Invalid verification parameters" });
            }

            try
            {
                var verificationToken = await _context.EmailVerificationTokens
                    .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == token && !t.IsUsed);

                if (verificationToken == null)
                {
                    return BadRequest(new { success = false, message = "Invalid or expired verification token" });
                }

                if (verificationToken.ExpiresAt < DateTime.UtcNow)
                {
                    return BadRequest(new { success = false, message = "Verification token has expired" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest(new { success = false, message = "User not found" });
                }

                user.IsEmailVerified = true;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                verificationToken.IsUsed = true;
                verificationToken.UsedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Email verified successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification");
                return StatusCode(500, new { success = false, message = "An error occurred during verification" });
            }
        }

        private async Task<string> GenerateEmailVerificationToken(string userId)
        {
            var token = GenerateSecureToken();
            var verificationToken = new EmailVerificationToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            _context.EmailVerificationTokens.Add(verificationToken);
            await _context.SaveChangesAsync();

            return token;
        }

        private string GenerateSecureToken()
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        private string GenerateMembershipNumber()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"MEM{timestamp}{random}";
        }
    }
}