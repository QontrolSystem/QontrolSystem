using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QontrolSystem.Data;
using QontrolSystem.Models;
using QontrolSystem.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QontrolSystem.Controllers.ControllersApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountControllerApi : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ServiceEmail _serviceEmail;
        private readonly IConfiguration _configuration;

        public AccountControllerApi(AppDbContext context, ServiceEmail serviceEmail, IConfiguration configuration)
        {
            _context = context;
            _serviceEmail = serviceEmail;
            _configuration = configuration;
        }

        // Register endpoint for API 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterValidation model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return Conflict("Email already exists.");
            }
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DepartmentID = model.DepartmentID,
                PasswordHash = HashPassword(model.PasswordHash),
                RoleID = 1, // Default to Employee
                CreatedAt = DateTime.Now,
                IsActive = false,
                IsApproved = false,
                IsRejected = false
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Send email notification
            await _serviceEmail.SendEmailAsync(
                user.Email,
                $"{user.FirstName} {user.LastName}",
                "Registration Successful",
                "<p>Your registration is pending approval.</p>"
            );
            return Ok("Registration successful. Awaiting approval.");
        }

        // Hash Password method using BCrypt
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Login endpoint for API
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users
                                .Include(u => u.Role)
                                .FirstOrDefault(u => u.Email == email);

            if (user == null || user.IsDeleted || !VerifyPassword(password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Check if user is System Administrator
            bool isAdmin = user.Role?.RoleName == "System Administrator";

            if (!isAdmin)
            {
                if (!user.IsActive)
                    return Forbid("Account is inactive. Please contact support.");

                if (!user.IsApproved)
                    return Forbid("Account not yet approved by admin.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "System Administrator"),
    };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = jwtToken,
                expires = tokenDescriptor.Expires
            });
        }


        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }
    }
}
