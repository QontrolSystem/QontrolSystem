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

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        public IActionResult Login()
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

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Role", user.Role.RoleName);

            // Redirect based on role
            switch (user.Role.RoleName)
            {
                case "System Administrator":
                    return RedirectToAction("Index", "UserManagement");
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

