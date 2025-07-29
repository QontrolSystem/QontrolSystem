using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.Accounts;
using QontrolSystem.Models.DataTransferObjectApi;

namespace QontrolSystem.Controllers.ControllersApis
{
    //[Route("api/[controller]")]
    [ApiController]
    public class Admin : ControllerBase
    {
        private readonly AppDbContext _context;

        public Admin(AppDbContext context)
        {
            _context = context;
        }


        // Endpoint to get pending user registrations
        [HttpGet("pending-users")]
        [Authorize(Roles = "System Administrator")]
        public IActionResult GetPendingRegistrations()
        {
            var pendingUsers = _context.Users
                .Include(u => u.Role)
                .Where(u => !u.IsApproved && !u.IsRejected)
                .Select(u => new
                {
                    u.UserID,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.PhoneNumber,
                    Department = u.Department.DepartmentName,
                    ITSubDepartmentID = u.ITSubDepartmentID,
                    ITSubDepartment = u.ITSubDepartment != null ? u.ITSubDepartment.SubDepartmentName : null,
                    Role = u.Role.RoleName,
                    u.CreatedAt
                })
                .ToList();
            return Ok(pendingUsers);
        }


        // Endpoint to approve a user registration 
        [HttpPut("approve-user")]
        [Authorize(Roles = "System Administrator")]
        public IActionResult ApproveUsers(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            user.IsActive = true;
            user.IsApproved = true;
            _context.SaveChanges();

            return Ok(new { message = $"User {id} approved successfully." });
        }

        //Create new user endpoint
        [HttpPost("create-user")]
        [Authorize(Roles = "System Administrator")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                return Conflict("A user with this email already exists.");
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DepartmentID = model.DepartmentID,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleID = model.RoleID,
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsApproved = true,
                IsRejected = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully", userId = user.UserID });
        }


        // Reject user endpoint
        [HttpPut("reject-user")]
        [Authorize(Roles = "System Administrator")]
        public IActionResult RejectUsers(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }
            user.IsRejected = true;
            user.IsActive = false;
            _context.SaveChanges();
            return Ok(new { message = $"User {id} rejected successfully." });

        }
        }
}
