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
            var userCount = _context.Users.Count();
            var technicianCount = _context.Users
                .Include(u => u.Role)
                .Count(u => u.Role.RoleName == "Technician");
            var departmentCount = _context.Departments.Count();

            var activeUsers = _context.Users.Count(u => u.IsActive);
            var inactiveUsers = _context.Users.Count(u => !u.IsActive);

            ViewBag.UserCount = userCount;
            ViewBag.TechnicianCount = technicianCount;
            ViewBag.DepartmentCount = departmentCount;

            ViewBag.ActiveUsers = activeUsers;
            ViewBag.InactiveUsers = inactiveUsers;

            return View();
        }
    }
}
