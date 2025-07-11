using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QontrolSystem.Data;
using QontrolSystem.Models;
using QontrolSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace QontrolSystem.Controllers.ControllersApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountControllerApi : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ServiceEmail _serviceEmail;

        public AccountControllerApi(AppDbContext context, ServiceEmail serviceEmail)
        {
            _context = context;
            _serviceEmail = serviceEmail;
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

    }
}
