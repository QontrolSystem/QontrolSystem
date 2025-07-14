using Microsoft.AspNetCore.Mvc;
using QontrolSystem.Models.Ticket;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;


namespace QontrolSystem.Controllers
{
    //[Authorize]
    public class TicketController : Controller
    {
        private readonly AppDbContext _context;

        public TicketController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Tickets()
        {
            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch all tickets created by the current user
            var tickets = _context.Tickets
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketUrgency)
                .Where(t => t.CreatedBy == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToList();

            return View(tickets);
        }


        // GET: Ticket/Create
        public IActionResult Create()
        {
            // Preload dropdown data
            ViewBag.TicketCategories = _context.TicketCategories.ToList();
            ViewBag.TicketUrgencies = _context.TicketUrgencies.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateTicketViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TicketCategories = _context.TicketCategories.ToList();
                ViewBag.TicketUrgencies = _context.TicketUrgencies.ToList();
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users
                               .Include(u => u.Department)
                               .FirstOrDefault(u => u.UserID == userId.Value);

            var ticket = new Ticket
            {
                Title = model.Title,
                Description = model.Description,
                TicketCategoryID = model.TicketCategoryID,
                TicketUrgencyID = model.TicketUrgencyID,
                CreatedBy = user.UserID,
                DepartmentID = user.DepartmentID,
                TicketStatusID = 1, // Open
                AssignedTo = user.UserID,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

            // Save attachments
            if (model.Attachments != null && model.Attachments.Any())
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                foreach (var file in model.Attachments)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var attachment = new TicketAttachment
                    {
                        TicketID = ticket.TicketID,
                        FilePath = "/uploads/" + fileName,
                        UploadedAt = DateTime.Now
                    };

                    _context.TicketAttachments.Add(attachment);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("Tickets", "Ticket"),
                duration = 3000,
                message = "Creating ticket...",
            });
        }     
    }
}

