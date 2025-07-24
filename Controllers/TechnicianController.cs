using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using QontrolSystem.Models.ViewModels;


namespace QontrolSystem.Controllers
{
    public class TechnicianController : Controller
    {

        private readonly AppDbContext _context;
        public TechnicianController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var now = DateTime.Now;

            // Get tickets assigned to this technician
            var tickets = _context.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketUrgency)
                .Where(t => t.AssignedTo == userId)
                .ToList();

            // If no tickets found, create dummy tickets for testing
            if (!tickets.Any())
            {
                tickets = new List<Models.Ticket.Ticket>
        {
            new Models.Ticket.Ticket
            {
                TicketID = 1,
                Title = "Dummy Ticket 1",
                TicketStatus = new Models.Ticket.TicketStatus { StatusName = "Open" },
                TicketCategory = new Models.Ticket.TicketCategory { CategoryName = "Software" },
                TicketUrgency = new Models.Ticket.TicketUrgency { UrgencyLevel = "High" },
                CreatedAt = now.AddDays(-2),
            },
            new Models.Ticket.Ticket
            {
                TicketID = 2,
                Title = "Dummy Ticket 2",
                TicketStatus = new Models.Ticket.TicketStatus { StatusName = "In Progress" },
                TicketCategory = new Models.Ticket.TicketCategory { CategoryName = "Hardware" },
                TicketUrgency = new Models.Ticket.TicketUrgency { UrgencyLevel = "Medium" },
                CreatedAt = now.AddDays(-5),
            }
        };
            }

            // Map to TechnicianTicket view models
            var assignedTickets = tickets.Select(t => new TechnicianTicket
            {
                TicketID = t.TicketID,
                Title = t.Title,
                Status = t.TicketStatus.StatusName,
                Category = t.TicketCategory?.CategoryName ?? "N/A",
                Urgency = t.TicketUrgency?.UrgencyLevel ?? "N/A",
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd"),
                DaysOpen = (now - t.CreatedAt).Days
            }).ToList();

            // Build dashboard view model
            var dashboard = new TechnicianDashboard
            {
                TotalAssignedTickets = assignedTickets.Count,
                OpenTickets = assignedTickets.Count(t => t.Status == "Open"),
                InProgressTickets = assignedTickets.Count(t => t.Status == "In Progress"),
                ResolvedTickets = assignedTickets.Count(t => t.Status == "Resolved"),
                OverdueTickets = assignedTickets.Count(t => t.DaysOpen > 7), // example
                AssignedTickets = assignedTickets
            };

            return View(dashboard);
        }


        public IActionResult TicketDetails(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            var ticket = _context.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketCategory)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .Include(t => t.Department)
                .Include(t => t.TicketUrgency)
                .FirstOrDefault(t => t.TicketID == id && t.AssignedTo == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        public IActionResult AssignedTickets()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var now = DateTime.Now;

            var tickets = _context.Tickets
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketUrgency)
                .Where(t => t.AssignedTo == userId)
                .ToList();

            var ticketList = tickets.Select(t => new TechnicianTicket
            {
                TicketID = t.TicketID,
                Title = t.Title,
                Status = t.TicketStatus.StatusName,
                Category = t.TicketCategory?.CategoryName ?? "N/A",
                Urgency = t.TicketUrgency?.UrgencyLevel ?? "N/A",
                CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd"),
                DaysOpen = (now - t.CreatedAt).Days
            }).OrderByDescending(t => t.DaysOpen).ToList();

            return View(ticketList);
        }


        // GET: Show form to update ticket status

        public IActionResult UpdateStatus(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var ticket = _context.Tickets
                .Include(t => t.TicketStatus)
                .FirstOrDefault(t => t.TicketID == id && t.AssignedTo == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            // Get all possible statuses for dropdown
            var statuses = _context.TicketStatuses
                .Select(s => new SelectListItem
                {
                    Value = s.TicketStatusID.ToString(),
                    Text = s.StatusName
                })
                .ToList();

            ViewBag.Statuses = statuses;

            return View(ticket);
        }

        // POST: Save updated status and optional notes
        [HttpPost]

        public IActionResult UpdateStatus(int id, int ticketStatusId, string workNotes)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var ticket = _context.Tickets
                .FirstOrDefault(t => t.TicketID == id && t.AssignedTo == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            ticket.TicketStatusID = ticketStatusId;

            //// Optional: Save work notes to ticket comments or logs
            //if (!string.IsNullOrWhiteSpace(workNotes))
            //{
            //    // Assuming you have a TicketComments table
            //    _context.TicketComments.Add(new Models.TicketComment
            //    {
            //        TicketID = id,
            //        Comment = workNotes,
            //        CreatedAt = DateTime.Now,
            //        AuthorID = userId
            //    });
            //}

            if (ticketStatusId == 3)
            {
                ticket.ResolvedAt = DateTime.Now;
            }
            else
            {
                ticket.ResolvedAt = null;
            }

            _context.SaveChanges();

            return RedirectToAction(nameof(TicketDetails), new { id });
        }

        //// GET: Show work logs for a ticket
        //[Authorize(Roles = "Technician")]
        //public IActionResult WorkLogs(int ticketId)
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //    // Ensure ticket belongs to this technician
        //    var ticket = _context.Tickets
        //        .Include(t => t.TicketComments)
        //        .ThenInclude(c => c.Author)
        //        .FirstOrDefault(t => t.TicketID == ticketId && t.AssignedTo == userId);

        //    if (ticket == null)
        //        return NotFound();

        //    return View(ticket);
        //}

        //// POST: Add a new work log comment
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Technician")]
        //public IActionResult AddWorkLog(int ticketId, string comment)
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        //    var ticket = _context.Tickets
        //        .FirstOrDefault(t => t.TicketID == ticketId && t.AssignedTo == userId);

        //    if (ticket == null)
        //        return NotFound();

        //    if (!string.IsNullOrWhiteSpace(comment))
        //    {
        //        var workLog = new TicketComment
        //        {
        //            TicketID = ticketId,
        //            AuthorID = userId,
        //            Comment = comment,
        //            CreatedAt = DateTime.Now
        //        };
        //        _context.TicketComments.Add(workLog);
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction(nameof(WorkLogs), new { ticketId = ticketId });
        //}

    }
}