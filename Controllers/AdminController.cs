using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;

namespace QontrolSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var filteredUser = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => u.IsApproved && !u.IsDeleted)
                .ToList();

            var userCount = filteredUser.Count();

            var departmentData = _context.Users
                .Where(u => u.IsApproved && u.IsActive)
                .Include(u => u.Department)
                .GroupBy(u => u.Department.DepartmentName)
                .Select(g => new
                {
                    DepartmentName = g.Key ?? "Unassigned",
                    Count = g.Count()
                })
                .ToList();

            ViewBag.DepartmentLabels = departmentData.Select(d => d.DepartmentName).ToList();
            ViewBag.DepartmentCounts = departmentData.Select(d => d.Count).ToList();

            var technicianCount = _context.Users
                .Include(u => u.Role)
                .Count(u => u.Role.RoleName == "Technician");
            var departmentCount = _context.Departments.Count();

            var activeUsers = _context.Users.Count(u => u.IsActive);
            var inactiveUsers = _context.Users.Count(u => !u.IsActive);
            
            ViewBag.PendingUserCount = _context.Users.Count(u => !u.IsApproved && !u.IsRejected);
            ViewBag.UserCount = userCount;
            ViewBag.TechnicianCount = technicianCount;
            ViewBag.DepartmentCount = departmentCount;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.InactiveUsers = inactiveUsers;

            return View();
        }

        public IActionResult ApproveUsers()
        {
            var pendingUsers = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Where(u => !u.IsApproved && !u.IsRejected)
                .ToList();

            return View(pendingUsers);
        }

        [HttpPost]
        public IActionResult Approve(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsActive = true;
                user.IsApproved = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ApproveUsers");
        }

        [HttpPost]
        public IActionResult Reject(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.IsRejected = true;
                _context.SaveChanges();
            }
            return RedirectToAction("ApproveUsers");
        }

    }
}
