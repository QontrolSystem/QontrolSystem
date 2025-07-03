using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models;
using QontrolSystem.Services;

namespace QontrolSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ServiceEmail _serviceEmail;


        public AccountController(AppDbContext context, ServiceEmail _serviceEmail)
        {
            _context = context;
            this._serviceEmail = _serviceEmail;
        }

        public IActionResult Register()
        {
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        [HttpPost]

        // The Register method handles user registration by validating the input data, checking for existing users, and saving the new user to the database.
        // The parameter has changed from Using the user model to RegisterValidation model to ensure that the input data is validated correctly.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterValidation model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                ViewBag.Departments = _context.Departments.ToList();
                return View(model);
            }

<<<<<<< HEAD
            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DepartmentID = model.DepartmentID,
                PasswordHash = HashPassword(model.PasswordHash),
                RoleID = 1,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
=======
            user.PasswordHash = HashPassword(user.PasswordHash);
            user.RoleID = 1; 
            user.CreatedAt = DateTime.Now;
            user.IsApproved = false; 
            user.IsRejected = false;
>>>>>>> 524c99e (User Approval Functionality)

            _context.Users.Add(user);
            _context.SaveChanges();

<<<<<<< HEAD

            // ✅ Send email to the email provided by the user
            await _serviceEmail.SendEmailAsync(
                model.Email,
                $"{model.FirstName} {model.LastName}",
                "Welcome to QontrolSystem!",
                $@"
            <p>Dear {model.FirstName},</p>
            <p>Thank you for registering on QontrolSystem.</p>
            <p>We're excited to have you on board!</p>
            <p>Please note that your account is currently inactive. An administrator will review your registration and activate your account shortly.</p>
            <br/>
            <p>Best regards,<br/>QontrolSystem Team</p>
        "
            );

            Console.WriteLine("✅ Email sent successfully to " + model.Email);

            TempData["Success"] = "Registration successful! Please check your email for confirmation.";
            return RedirectToAction("Login");

            TempData["Success"] = "Registration successful! You can now log in.";
            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("Login", "Account"),
                duration = 3000,
                message = "Redirecting to login",
            });

        }


=======
            TempData["Info"] = "Registration submitted! Awaiting admin approval.";
            return RedirectToAction("PendingApproval");
        }

        public IActionResult PendingApproval()
        {
            return View();
        }
>>>>>>> 524c99e (User Approval Functionality)


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
                               .FirstOrDefault(u => u.Email == email);

            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

<<<<<<< HEAD
            if(!user.IsActive)
            {
                
                return View("PendingApproval");
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Role", user.Role.RoleName);

            string? targetUrl = user.Role.RoleName switch
=======
            
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
>>>>>>> 524c99e (User Approval Functionality)
            {
                "System Administrator" => Url.Action("Dashboard", "Admin"),
                "Technician" => Url.Action("Index", "TechnicianDashboard"),
                "IT Manager" => Url.Action("Index", "ManagerDashboard"),
                _ => Url.Action("Index", "Home")
            };

            // Redirect to loading screen first
            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = targetUrl ?? Url.Action("Index", "Home"),
                duration = 3000,
                message = "Loading your dashboard",
            });
        }




        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
         
            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("Login", "Account"),
                duration = 3000,
                message = "Logging out",
            });


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

            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("Profile", "Account"),
                duration = 3000,
                message = "Loading your profile",
            });

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

