using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.ViewModels;
using System.Linq;

namespace QontrolSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tickets = _context.Tickets
                .Include(t => t.TicketStatus)
                .Where(t => t.CreatedBy == userId)
                .ToList();

            var dashboard = new EmployeeDashboard
            {
                TotalTicketsLogged = tickets.Count,
                OpenTickets = tickets.Count(t => t.TicketStatus.StatusName == "Open"),
                InProgressTickets = tickets.Count(t => t.TicketStatus.StatusName == "In Progress"),
                ResolvedTickets = tickets.Count(t => t.TicketStatus.StatusName == "Resolved"),
                ClosedTickets = tickets.Count(t => t.TicketStatus.StatusName == "Closed")
            };

            return View(dashboard); // View: Views/Employee/Dashboard.cshtml
        }
    }
}

