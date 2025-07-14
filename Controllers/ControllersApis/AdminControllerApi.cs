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
                    Role = u.Role.RoleName,
                    u.CreatedAt
                })
                .ToList();
            return Ok(pendingUsers);
        }

    }
}
