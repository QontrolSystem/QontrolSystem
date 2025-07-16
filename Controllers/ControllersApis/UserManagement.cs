using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.DataTransferObjectApi;
using QontrolSystem.Models.ViewModels;

namespace QontrolSystem.Controllers.ControllersApis
{
    //[Route("api/[controller]")]
    [ApiController]
    public class UserManagement : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserManagement(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "System Administrator";
        }

        //View Users List
        [HttpGet("view-users")]
        [Authorize(Roles ="System Administrator")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.IsApproved && !u.IsDeleted)
                .Select(u => new UserListDto
                {
                    UserID = u.UserID,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    RoleName = u.Role.RoleName,
                    DepartmentName = u.Department.DepartmentName,
                    IsActive = u.IsActive
                })
                .ToList();

            return Ok(users);
        }



        [HttpGet("retrive-user-details-to-edit/{id}")]
        [Authorize(Roles = "System Administrator")]
        public IActionResult GetUserById(int id)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .FirstOrDefault(u => u.UserID == id && !u.IsDeleted);

            if (user == null)
                return NotFound(new { message = "User not found" });

            // Map to DTO for safety
            var userDto = new UserEditDto
            {
                UserID = user.UserID,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                RoleID = user.RoleID,
                DepartmentID = user.DepartmentID,
                IsActive = user.IsActive
                // NewPassword is not included for security — only used during update
            };

            return Ok(userDto);
        }

        //Update User Details API
        [HttpPut("update-user")]
        [Authorize(Roles = "System Administrator")]
        public IActionResult UpdateUser([FromBody] UserEditDto updatedUser)
        {
            var user = _context.Users.Find(updatedUser.UserID);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Update fields
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.RoleID = updatedUser.RoleID;
            user.DepartmentID = updatedUser.DepartmentID;
            user.IsActive = updatedUser.IsActive;

            if (!string.IsNullOrEmpty(updatedUser.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updatedUser.NewPassword);

            _context.SaveChanges();

            return Ok(new { message = "User updated successfully" });
        }


        //Soft Delete API
        [HttpDelete("delete-user/{id}")]
        [Authorize(Roles = "System Administrator")]
        public IActionResult SoftDeleteUser(int id)
        {
            // Optional role check for System Admins only
            var role = HttpContext.Session.GetString("Role");
            if (role != "System Administrator")
                return Forbid();

            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Perform soft delete
            user.IsDeleted = true;
            user.IsActive = false;

            _context.Users.Update(user);
            _context.SaveChanges();

            return Ok(new { message = $"User with ID {id} has been soft-deleted." });
        }
    }
}
