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
                AssignedTo = null,
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
                message = "Creating ticket",
            });
        }

        public IActionResult TicketDetail(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketUrgency)
                .Include(t => t.TicketAttachments)
                .Include(t => t.Creator)
                .Include(t => t.Assignee)
                .FirstOrDefault(t => t.TicketID == id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // GET
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var ticket = _context.Tickets
                .Include(t => t.TicketAttachments)
                .FirstOrDefault(t => t.TicketID == id && t.CreatedBy == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Ticket ticket, List<IFormFile>? NewAttachments)
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var existingTicket = _context.Tickets
                .Include(t => t.TicketAttachments)
                .FirstOrDefault(t => t.TicketID == ticket.TicketID && t.CreatedBy == userId);

            if (existingTicket == null)
            {
                return NotFound();
            }

            // Update description
            existingTicket.Description = ticket.Description;
            existingTicket.UpdatedAt = DateTime.Now;

            // Handle new attachments
            if (NewAttachments != null && NewAttachments.Any())
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                foreach (var file in NewAttachments)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var attachment = new TicketAttachment
                    {
                        TicketID = existingTicket.TicketID,
                        FilePath = "/uploads/" + fileName,
                        UploadedAt = DateTime.Now
                    };

                    _context.TicketAttachments.Add(attachment);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index", "Loading", new
            {
                returnUrl = Url.Action("Tickets", "Ticket"),
                duration = 3000,
                message = "Updating ticket",
            });
        }     

    }
}

