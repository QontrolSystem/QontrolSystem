using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Controllers
{
    public class ManagerController : Controller
    {

        private readonly AppDbContext _context;

        public ManagerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        private List<Ticket> GetTeamTickets(int managerId)
        {
            var manager = _context.Users
                .Include(u => u.ITSubDepartment)
                .FirstOrDefault(u => u.UserID == managerId);

            if (manager == null || manager.ITSubDepartmentID == null)
            {
                return new List<Ticket>(); // No tickets for managers without a sub-department
            }

            var managerSubDeptName = manager.ITSubDepartment.SubDepartmentName;

            var tickets = _context.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketCategory)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Department)
                .Include(t => t.TicketUrgency)
                .Where(t => t.TicketCategory.CategoryName == managerSubDeptName)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return tickets;
        }

        public IActionResult AllTickets()
        {

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tickets = GetTeamTickets(userId.Value);

            return View(tickets);
        }

        public IActionResult ManageTeams()
        {
            var teams = _context.ITSubDepartments
                .Include(t => t.Department)
                .Select(t => new ManageTeams
                {
                    ITSubDepartmentID = t.ITSubDepartmentID,
                    SubDepartmentName = t.SubDepartmentName,
                    DepartmentName = t.Department.DepartmentName,
                    TechnicianNames = _context.Users
                        .Where(u => u.ITSubDepartmentID == t.ITSubDepartmentID && u.RoleID == 2 && u.IsActive)
                        .Select(u => u.FirstName + " " + u.LastName)
                        .ToList()
                })
                .ToList();

            return View(teams);
        }
        public IActionResult ManageTechnicians()
        {

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var manager = _context.Users
                .Include(u => u.ITSubDepartment)
                .FirstOrDefault(u => u.UserID == userId.Value);

            if (manager == null || manager.ITSubDepartmentID == null)
            {
                return Forbid(); // Manager has no sub-department
            }

            var managerSubDeptId = manager.ITSubDepartmentID;

            var technicians = _context.Users
                .Include(u => u.ITSubDepartment)
                .Where(u => u.RoleID == 2 // Technicians
                        && u.ITSubDepartmentID == managerSubDeptId)
                .OrderBy(u => u.FirstName)
                .Select(u => new ManageTechnicians
                {
                    UserID = u.UserID,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Team = u.ITSubDepartment != null ? u.ITSubDepartment.SubDepartmentName : "Unassigned",
                    IsActive = u.IsActive,
                    IsApproved = u.IsApproved,
                    CreatedAt = u.CreatedAt
                })
                .ToList();

            return View(technicians);
        }

        public IActionResult UnassignedTickets()
        {

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var tickets = GetTeamTickets(userId.Value);
            var unassignedTickets = tickets
                .Where(t => t.TicketStatus.StatusName == "Open" && (t.AssignedTo == null || t.AssignedTo == 0))
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return View(unassignedTickets);
        }

        public IActionResult TicketDetails(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketCategory)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Department)
                .Include(t => t.TicketUrgency)
                //.Include(t => t.TicketComments) // if you have comments or logs related
                .FirstOrDefault(t => t.TicketID == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }
    }
}

