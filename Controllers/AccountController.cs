using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Enums;
using QontrolSystem.Models.Accounts;
using QontrolSystem.Models.ViewModels;
using QontrolSystem.Services;
using QontrolSystem.Utils;

namespace QontrolSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ServiceEmail _serviceEmail;

        public AccountController(AppDbContext context, ServiceEmail serviceEmail)
        {
            _context = context;
            _serviceEmail = serviceEmail;
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();
            return View();
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterValidation model)
        {

            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();

            if (!ModelState.IsValid)
            {
                ViewBag.Departments = _context.Departments.ToList();
                ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already exists.");
                ViewBag.Departments = _context.Departments.ToList();
                ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DepartmentID = model.DepartmentID,
                ITSubDepartmentID = model.ITSubDepartmentID,
                PasswordHash = HashPassword(model.PasswordHash),
                RoleID = 1, // Default to Employee
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsApproved = false,
                IsRejected = false
            };


            user.IsActive = false;

            var selectedDept = _context.Departments.FirstOrDefault(d => d.DepartmentID == model.DepartmentID);
            if (selectedDept != null && selectedDept.DepartmentName == "IT Department" && model.ITSubDepartmentID == null)
            {
                ModelState.AddModelError("ITSubDepartmentID", "Please select an IT Sub-Department.");
                ViewBag.Departments = _context.Departments.ToList();
                ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();
                return View(model); // ✅ Always return RegisterValidation model
            }


            _context.Users.Add(user);
            _context.SaveChanges();


            // ✅ Send email to user

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
                <p>Best regards,<br/>QontrolSystem Team</p>"
            );

            Console.WriteLine("✅ Email sent successfully to " + model.Email);

            TempData["Info"] = "Registration submitted! Awaiting admin approval.";
            return RedirectToAction("PendingApproval");
        }

        public IActionResult PendingApproval()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult DeactivatedUser()
        {
            return View();
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        private string GenerateOtp(int length)
        {
            var random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                                         .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model)
        {
            var otp = GenerateOtp(6);

            var otpEntity = new PasswordResetOtp
            {
                Email = model.Email,
                OtpCode = otp,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };

            _context.PasswordResetOtps.Add(otpEntity);
            await _context.SaveChangesAsync();

            await _serviceEmail.SendEmailAsync(
                model.Email,
                "User",
                "Password Reset OTP",
                $@"
            <p>Hello,</p>
            <p>Please use this OTP to reset your password: <strong>{otp}</strong></p>
            <p>This OTP expires in 10 minutes.</p>"
            );

            ViewBag.Message = "An OTP has been sent to your email.";
            return RedirectToAction("ResetPassword");
        }

        [HttpPost("Login")]
        public IActionResult Login(Login model)
        {
            var user = _context.Users
                               .Include(u => u.Role)
                               .FirstOrDefault(u => u.Email == model.Email);

            if (user == null || user.IsDeleted || !VerifyPassword(model.Password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }

            
            var roleEnum = EnumHelper.GetValueFromDisplayName<UserRole>(user.Role.RoleName);
            bool isAdmin = roleEnum == UserRole.SystemAdministrator;


            if (!isAdmin)
            {
                if (user.IsRejected)
                {
                    return RedirectToAction("AccessDenied");
                }

                if (!user.IsApproved)
                {
                    return RedirectToAction("PendingApproval");
                }

                if(!user.IsActive)
                {
                    return RedirectToAction("DeactivatedUser");
                }
            }

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetString("Role", user.Role.RoleName);
            HttpContext.Session.SetString("UserName", user.FirstName);

            string? targetUrl = user.Role.RoleName switch
            {
                "System Administrator" => Url.Action("Dashboard", "Admin"),
                "Technician" => Url.Action("Index", "TechnicianDashboard"),
                "IT Manager" => Url.Action("Dashboard", "ITManager"),
                _ => Url.Action("Index", "Home")
            };

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
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}

            // Check if OTP is valid
            var otpRecord = await _context.PasswordResetOtps
                .FirstOrDefaultAsync(x =>
                    x.Email == model.Email &&
                    x.OtpCode == model.OtpCode &&
                    !x.IsUsed &&
                    x.ExpiresAt > DateTime.UtcNow
                );

            if (otpRecord == null)
            {
                ModelState.AddModelError("", "Invalid or expired OTP.");
                return View(model);
            }

            // Mark OTP as used
            otpRecord.IsUsed = true;
            await _context.SaveChangesAsync();

            // Update the user's password
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            user.PasswordHash = HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Password reset successfully!";
            return RedirectToAction("Login");
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
