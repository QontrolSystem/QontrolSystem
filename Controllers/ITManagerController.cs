using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Controllers
{
    public class ITManagerController : Controller
    {
        private readonly AppDbContext _context;

        public ITManagerController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var managerId = HttpContext.Session.GetInt32("UserID");
            if (managerId == null)
                return RedirectToAction("Login", "Account");

            var tickets = await GetTeamTicketsAsync(managerId.Value);

            return View(tickets); 
        }

        private async Task<List<Tickets>> GetTeamTicketsAsync(int managerId)
        {
            var manager = await _context.Users
                .Include(u => u.ITSubDepartment)
                .FirstOrDefaultAsync(u => u.UserID == managerId);

            if (manager == null || manager.ITSubDepartmentID == null)
            {
                return new List<Tickets>();
            }

            var subDeptId = manager.ITSubDepartmentID.Value;

            var tickets = await _context.Tickets
                .AsNoTracking()
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketCategory)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Department)
                .Include(t => t.TicketUrgency)
                .Where(t => t.TicketCategory != null
                            && t.TicketCategory.SubDepartmentID == subDeptId) 
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tickets;
        }

    }
}
