using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Enums;
using QontrolSystem.Models.Accounts;
using QontrolSystem.Models.ViewModels;

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


        public IActionResult UserManagementIndex(string searchString, string roleFilter, string departmentFilter, string isActiveFilter, int page = 1, int pageSize = 4)
        {
            if (!IsAdmin())
                return RedirectToAction("AccessDenied", "Account");

            var usersQuery = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.IsApproved && !u.IsDeleted)
                .AsQueryable();
          
            if (!string.IsNullOrEmpty(searchString))
            {
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.Contains(searchString) ||
                    u.LastName.Contains(searchString) ||
                    u.Email.Contains(searchString) ||
                    u.Role.RoleName.Contains(searchString) ||
                    u.Department.DepartmentName.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(roleFilter))
                usersQuery = usersQuery.Where(u => u.Role.RoleName == roleFilter);

            if (!string.IsNullOrEmpty(departmentFilter))
                usersQuery = usersQuery.Where(u => u.Department.DepartmentName == departmentFilter);

            if (!string.IsNullOrEmpty(isActiveFilter) && bool.TryParse(isActiveFilter, out bool isActive))
                usersQuery = usersQuery.Where(u => u.IsActive == isActive);

            // Pagination
            int totalUsers = usersQuery.Count();
            var users = usersQuery
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Roles = _context.Roles.Select(r => r.RoleName).Distinct().ToList();
            ViewBag.Departments = _context.Departments.Select(d => d.DepartmentName).Distinct().ToList();

            var viewModel = new UserList
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                SearchString = searchString,
                RoleFilter = roleFilter,
                DepartmentFilter = departmentFilter,
                IsActiveFilter = isActiveFilter
            };

            return View(viewModel);
        }



        public IActionResult Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            if (id == null) return NotFound();

            var user = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Include(u => u.ITSubDepartment)
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
            ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();

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

        public IActionResult Edit(int? id, string returnUrl = null)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
            if (id == null) return NotFound();

            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            ViewBag.Roles = _context.Roles.ToList();
            ViewBag.Departments = _context.Departments.ToList();
            ViewBag.ITSubDepartments = _context.ITSubDepartments.ToList();


            ViewBag.ReturnUrl = returnUrl ?? Url.Action("Dashboard", "Admin"); 
           

            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User updatedUser, string? NewPassword, string returnUrl = null)
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
                returnUrl = returnUrl ?? Url.Action("UserManagementIndex", "UserManagement"),
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

        [HttpGet("Technicians")]
        public IActionResult Technicians()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            var approvedTechnicians = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Include(u => u.ITSubDepartment)
                .Where(u =>
                    u.Role.RoleName == "Technician" &&
                    u.IsApproved &&
                    !u.IsDeleted)
                .ToList();

            return View(approvedTechnicians);
        }

        public IActionResult Technicians(string searchString, string departmentFilter, string isActiveFilter, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Users
                .Include(u => u.ITSubDepartment)
                .Where(u => u.Role.RoleName == "Technician" && u.IsApproved)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(searchString) ||
                    u.LastName.Contains(searchString) ||
                    u.Email.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(departmentFilter))
            {
                query = query.Where(u => u.ITSubDepartment.SubDepartmentName == departmentFilter);
            }

            if (!string.IsNullOrEmpty(isActiveFilter))
            {
                bool isActive = isActiveFilter == "true";
                query = query.Where(u => u.IsActive == isActive);
            }

            int totalUsers = query.Count();
            var users = query
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new UserList
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                SearchString = searchString,
                DepartmentFilter = departmentFilter,
                IsActiveFilter = isActiveFilter
            };

            ViewBag.SearchString = searchString;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.IsActiveFilter = isActiveFilter;
            ViewBag.Departments = _context.ITSubDepartments
                                         .Select(d => d.SubDepartmentName)
                                         .Distinct()
                                         .ToList();


            return View(viewModel);
        }



        [HttpGet("Managers")]
        public IActionResult Managers()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");

            var approvedManagers = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Include(u => u.ITSubDepartment)
                .Where(u =>
                    u.Role.RoleName == "IT Manager" &&
                    u.IsApproved &&
                    !u.IsDeleted)
                .ToList();

            return View(approvedManagers);
        }

        public IActionResult Managers(string searchString, string departmentFilter, string isActiveFilter, int page = 1)
        {
            int pageSize = 10;

            var query = _context.Users
                .Include(u => u.ITSubDepartment)
                .Where(u => u.Role.RoleName == "IT Manager" && u.IsApproved)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(u =>
                    u.FirstName.Contains(searchString) ||
                    u.LastName.Contains(searchString) ||
                    u.Email.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(departmentFilter))
            {
                query = query.Where(u => u.ITSubDepartment.SubDepartmentName == departmentFilter);
            }

            if (!string.IsNullOrEmpty(isActiveFilter))
            {
                bool isActive = isActiveFilter == "true";
                query = query.Where(u => u.IsActive == isActive);
            }

            int totalUsers = query.Count();
            var users = query
                .OrderBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new UserList
            {
                Users = users,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize),
                SearchString = searchString,
                DepartmentFilter = departmentFilter,
                IsActiveFilter = isActiveFilter
            };

            ViewBag.SearchString = searchString;
            ViewBag.DepartmentFilter = departmentFilter;
            ViewBag.IsActiveFilter = isActiveFilter;
            ViewBag.Departments = _context.ITSubDepartments
                                         .Select(d => d.SubDepartmentName)
                                         .Distinct()
                                         .ToList();


            return View(viewModel);
        }

    }
}

