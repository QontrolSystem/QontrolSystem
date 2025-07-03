using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models;

namespace QontrolSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Register()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                ViewBag.Departments = _context.Departments.ToList();
                return View(user);
            }

            user.PasswordHash = HashPassword(user.PasswordHash);
            user.RoleID = 1; 
            user.CreatedAt = DateTime.Now;
            user.IsApproved = false; 
            user.IsRejected = false;

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Info"] = "Registration submitted! Awaiting admin approval.";
            return RedirectToAction("PendingApproval");
        }

        public IActionResult PendingApproval()
        {
            return View();
        }


        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users
                               .Include(u => u.Role)
                               .FirstOrDefault(u => u.Email == email && u.IsActive);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            
            bool isAdmin = user.Role.RoleName == "System Administrator";

            if (!isAdmin)
            {
                if (user.IsRejected)
                {
                    return RedirectToAction("AccessDenied", "Account");
                }

                if (!user.IsApproved)
                {
                    return RedirectToAction("PendingApproval", "Account");
                }
            }

            // Store session values
            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Role", user.Role.RoleName);

            // Role-based redirect
            switch (user.Role.RoleName)
            {
                case "System Administrator":
                    return RedirectToAction("Dashboard", "Admin");
                case "Technician":
                    return RedirectToAction("Index", "TechnicianDashboard");
                case "IT Manager":
                    return RedirectToAction("Index", "ManagerDashboard");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();

            return View(user);
        }

        [HttpPost]
        public IActionResult Profile(User updatedUser, string? NewPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.Users.Find(userId);
            if (user == null) return NotFound();

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.PhoneNumber = updatedUser.PhoneNumber;

            if (!string.IsNullOrEmpty(NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(NewPassword);

            _context.SaveChanges();
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
        }

    }
}

