using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models;

namespace QontrolSystem.Controllers
{
    public class UserManagementController : Controller
    {
        private readonly AppDbContext _context;

        public UserManagementController(AppDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "System Administrator";
        }


        public IActionResult UserManagementIndex(string searchString, string roleFilter, string departmentFilter, string isActiveFilter)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var users = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u =>  u.IsActive && u.IsApproved && !u.IsDeleted) 
                .AsQueryable();

            ViewBag.Roles = _context.Roles.Select(r => r.RoleName).Distinct().ToList();
            ViewBag.Departments = _context.Departments.Select(d => d.DepartmentName).Distinct().ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u =>
                    u.FirstName.Contains(searchString) ||
                    u.LastName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.Role.RoleName.Contains(searchString) ||
                    u.Department.DepartmentName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role.RoleName == roleFilter);
            }

            if (!string.IsNullOrEmpty(departmentFilter))
            {
                users = users.Where(u => u.Department.DepartmentName == departmentFilter);
            }

            if (!string.IsNullOrEmpty(isActiveFilter))
            {
                bool isActive = bool.Parse(isActiveFilter);
                users = users.Where(u => u.IsActive == isActive);
            }

            return View(users.ToList());
        }



        public IActionResult Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            if (id == null) return NotFound();

            var user = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => !u.IsDeleted)
                .FirstOrDefault(u => u.UserID == id);

            if (user == null) return NotFound();

            return View(user);
        }

        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            ViewBag.Roles = _context.Roles.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(User user)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                return View(user);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.Now;

            _context.Users.Add(user);
            _context.SaveChanges();
          
            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("UserManagementIndex", "UserManagement"),
                duration = 3000,
                message = "Creating user",
            });
        }

        public IActionResult Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
            if (id == null) return NotFound();

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _context.Roles.ToList();
            ViewBag.Departments = _context.Departments.ToList();

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser, string? NewPassword)
        {
            var user = _context.Users.Find(updatedUser.UserID);
            if (user == null) return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;
            user.RoleID = updatedUser.RoleID;
            user.DepartmentID = updatedUser.DepartmentID;
            user.IsActive = updatedUser.IsActive;

            if (!string.IsNullOrEmpty(NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            _context.SaveChanges();
           
            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("UserManagementIndex", "UserManagement"),
                duration = 3000,
                message = "Saving changes",
            });
        }


        public IActionResult Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.UserID == id);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound();

            // Perform soft delete
            user.IsDeleted = true;
            user.IsActive = false;

            _context.Users.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("UserManagementIndex", "UserManagement"),
                duration = 3000,
                message = "Deleting user",
            });
        }


    }
}

