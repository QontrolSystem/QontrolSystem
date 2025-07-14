using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QontrolSystem.Data;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        public IActionResult GetPendingRegistrations()
        {
            var pendingUsers = _context.Users
                .Where(u => !u.IsApproved && !u.IsRejected && u.IsActive)
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
