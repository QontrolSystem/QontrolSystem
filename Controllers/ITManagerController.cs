using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.Ticket;
using QontrolSystem.Models.ViewModels;

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


        [Route("ITManager/_AssignTicketPopup")]
        public IActionResult _AssignTicketPopup(int ticketId)
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
                return Forbid();
            }

            var managerSubDeptId = manager.ITSubDepartmentID;

            var teamUsers = _context.Users
                .Include(u => u.ITSubDepartment)
                .Where(u => u.ITSubDepartmentID == managerSubDeptId)
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
     

            return PartialView("_AssignTicketPopup");
        }




        [HttpGet]
        public async Task<IActionResult> AssignPopup(int ticketId)
        {
            var managerId = HttpContext.Session.GetInt32("UserID");
            if (managerId == null)
                return RedirectToAction("Login", "Account");

            var manager = await _context.Users
                .Include(u => u.ITSubDepartment)
                .FirstOrDefaultAsync(u => u.UserID == managerId.Value);

            if (manager == null || manager.ITSubDepartmentID == null)
                return Forbid();

            var ticket = await _context.Tickets
                .Include(t => t.TicketCategory)
                .Include(t => t.Assignee)
                .FirstOrDefaultAsync(t => t.TicketID == ticketId);

            if (ticket == null)
                return NotFound();

            if (ticket.TicketCategory == null || ticket.TicketCategory.SubDepartmentID != manager.ITSubDepartmentID)
                return Forbid();

            var teamUsers = await _context.Users
                .Include(u => u.ITSubDepartment)
                .Where(u => u.ITSubDepartmentID == manager.ITSubDepartmentID)
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
                .ToListAsync();

            var vm = new AssignTicket
            {
                TicketID = ticket.TicketID,
                CurrentAssigneeId = ticket.AssignedTo,
                AssignmentDate = DateTime.Today, 
                TeamUsers = teamUsers
            };

            return PartialView("_AssignTicketPopup", vm);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTicket(AssignTicketInput input)
        {
            var managerId = HttpContext.Session.GetInt32("UserID");
            if (managerId == null)
                return RedirectToAction("Login", "Account");

            var manager = await _context.Users
                .Include(u => u.ITSubDepartment)
                .FirstOrDefaultAsync(u => u.UserID == managerId.Value);

            if (manager == null || manager.ITSubDepartmentID == null)
                return Forbid();

            var ticket = await _context.Tickets
                .Include(t => t.TicketCategory)
                .FirstOrDefaultAsync(t => t.TicketID == input.TicketID);

            if (ticket == null)
                return NotFound();

            if (ticket.TicketCategory == null || ticket.TicketCategory.SubDepartmentID != manager.ITSubDepartmentID)
                return Forbid();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == input.UserID);
            if (user == null || user.ITSubDepartmentID != manager.ITSubDepartmentID)
            {
                TempData["Error"] = "Selected user is not valid for assignment.";
                return RedirectToAction("Dashboard");
            }

            // Assign
            ticket.AssignedTo = user.UserID;
            // Optionally store the assignment date somewhere. If you want a dedicated field, consider adding one like ScheduledDate.
            // Example: ticket.UpdatedAt = input.AssignmentDate; // only if that fits your semantics

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Ticket #{ticket.TicketID} assigned to {user.FirstName} {user.LastName} for {input.AssignmentDate:yyyy-MM-dd}";

            return RedirectToAction("Dashboard");
        }

    }
}

