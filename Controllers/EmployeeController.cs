using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.ViewModels;
using QontrolSystem.Models.Ticket;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QontrolSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Employee/Dashboard
        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tickets = _context.Tickets
                .Where(t => t.AssignedTo == userId)
                .Include(t => t.TicketStatus)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            var viewModel = new EmployeeDashboardViewModel
            {
                TotalTickets = tickets.Count,
                InProgressTickets = tickets.Count(t => t.TicketStatus.StatusName == "In Progress"),
                ResolvedTickets = tickets.Count(t => t.TicketStatus.StatusName == "Resolved"),
                ClosedTickets = tickets.Count(t => t.TicketStatus.StatusName == "Closed"),
                RecentTickets = tickets.Take(5).ToList(),
                TicketStatusCounts = tickets
                    .GroupBy(t => t.TicketStatus.StatusName)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return View(viewModel);
        }
    }
}
