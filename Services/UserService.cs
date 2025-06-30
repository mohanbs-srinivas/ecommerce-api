using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ecommerce_api.Models;

namespace ecommerce_api.Services
{
    public class UserService
    {
        private readonly List<User> _users;

        public UserService()
        {
            _users = new List<User>();
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return Task.FromResult(_users);
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            return Task.FromResult(user);
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            var user = _users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(user);
        }

        public async Task<UserRegistrationResponseDto> RegisterUserAsync(UserRegistrationDto registrationDto)
        {
            // Check for duplicate email
            var existingUser = await GetUserByEmailAsync(registrationDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email address already exists.");
            }

            // Validate password strength
            if (!IsPasswordStrong(registrationDto.Password))
            {
                throw new ArgumentException("Password must contain at least one uppercase letter, one lowercase letter, and one number.");
            }

            // Create new user
            var newUser = new User
            {
                Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1,
                Name = registrationDto.Name,
                Email = registrationDto.Email,
                Password = HashPassword(registrationDto.Password), // In a real app, use proper hashing
                MembershipDate = DateTime.UtcNow,
                Role = "Member"
            };

            _users.Add(newUser);

            return new UserRegistrationResponseDto
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
                MembershipDate = newUser.MembershipDate,
                Role = newUser.Role,
                Message = "User registered successfully. A confirmation email will be sent shortly."
            };
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                _users.Remove(user);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        private bool IsPasswordStrong(string password)
        {
            // Password must contain at least one uppercase letter, one lowercase letter, and one number
            var hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
            var hasLowerCase = Regex.IsMatch(password, @"[a-z]");
            var hasNumber = Regex.IsMatch(password, @"[0-9]");
            
            return hasUpperCase && hasLowerCase && hasNumber;
        }

        private string HashPassword(string password)
        {
            // In a real application, use proper password hashing like BCrypt
            // For demo purposes, using simple base64 encoding
            var bytes = System.Text.Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(bytes);
        }
    }
}