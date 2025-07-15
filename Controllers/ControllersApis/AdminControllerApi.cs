using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QontrolSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace QontrolSystem.Controllers.ControllersApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminControllerApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminControllerApi(AppDbContext context)
        {
            _context = context;
        }


        // Endpoint to get pending user registrations
        [HttpGet("pending-registrations")]
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
