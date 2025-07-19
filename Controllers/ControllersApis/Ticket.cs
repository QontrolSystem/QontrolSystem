using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.Ticket;
using QontrolSystem.Data.TransferObjectApi;
using QontrolSystem.Models.ViewModels;


namespace QontrolSystem.Controllers.ControllersApis
{
    //[Route("api/[controller]")]
    [ApiController]
    public class Ticket : ControllerBase
    {
        private readonly AppDbContext _context;

        public Ticket(AppDbContext context)
        {
            _context = context;
        }


        //Endpoint to create a new ticket
        [HttpPost]
        [Route("create-ticket")]
        [Authorize(Roles = "Employee")]
        public IActionResult Create([FromForm] CreateTicket model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var user = _context.Users
                               .Include(u => u.Department)
                               .FirstOrDefault(u => u.UserID == userId);

           

            var ticket = new Tickets
            {
                Title = model.Title,
                Description = model.Description,
                TicketCategoryID = model.TicketCategoryID,
                TicketUrgencyID = model.TicketUrgencyID,
                CreatedBy = user.UserID,
                DepartmentID = user.DepartmentID,
                TicketStatusID = 1,
                AssignedTo = user.UserID,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Tickets.Add(ticket);
            _context.SaveChanges();

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

                    var attachment = new Attachment
                    {
                        TicketID = ticket.TicketID,
                        FilePath = "/uploads/" + fileName,
                        UploadedAt = DateTime.Now
                    };

                    _context.TicketAttachments.Add(attachment);
                }
                _context.SaveChanges();
            }

            return Ok(new
            {
                returnUrl = Url.Action("Tickets", "Ticket"),
                duration = 3000,
                message = "Creating ticket",
            });
        }


        // Get all tickets for the authenticated user
        [HttpGet]
        [Route("view-tickets")]
        [Authorize(Roles = "Employee")]
        public IActionResult Tickets()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var tickets = _context.Tickets
          .Include(t => t.TicketCategory)
          .Include(t => t.TicketStatus)
          .Include(t => t.TicketUrgency)
          .Where(t => t.CreatedBy == userId)
          .OrderByDescending(t => t.CreatedAt)
          .Select(t => new Data.TransferObjectApi.Ticket
          {
          TicketID = t.TicketID,
          Title = t.Title,
          Description = t.Description,
          TicketCategoryID = t.TicketCategory.TicketCategoryID,
          TicketStatusID = t.TicketStatus.TicketStatusID,
          TicketUrgencyID = t.TicketUrgency.TicketUrgencyID,
          CreatedBy = t.CreatedBy,
          CreatedAt = t.CreatedAt
          })
          .ToList();

            return Ok(tickets);
        }


        // Get ticket details by ID
        [HttpGet]
        [Route("retrive-ticket/{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Tickets(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var ticket = _context.Tickets
                .Include(t => t.TicketCategory)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketUrgency)
                .Where(t => t.CreatedBy == userId && t.TicketID == id)
                .Select(t => new Data.TransferObjectApi.Ticket
                {
                    TicketID = t.TicketID,
                    Title = t.Title,
                    Description = t.Description,
                    TicketCategoryID = t.TicketCategory.TicketCategoryID,
                    TicketStatusID = t.TicketStatus.TicketStatusID,
                    TicketUrgencyID = t.TicketUrgency.TicketUrgencyID,
                    CreatedBy = t.CreatedBy,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefault();

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }


        // Endpoint to get the ticket to be edited
        [HttpGet]
        [Route("retrive-ticket-to-edit/{id}")]
        [Authorize(Roles = "Employee")]
        public IActionResult Edit(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var ticket = _context.Tickets
                .Include(t => t.TicketAttachments)
                .FirstOrDefault(t => t.TicketID == id && t.CreatedBy == userId);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }


        // Endpoint to update a ticket
        [HttpPost]
        [Route("edit-ticket")]
        [Authorize(Roles = "Employee")]
        public IActionResult Edit([FromForm] Edit model, [FromForm] List<IFormFile>? NewAttachments)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var existingTicket = _context.Tickets
                .Include(t => t.TicketAttachments)
                .FirstOrDefault(t => t.TicketID == model.TicketID && t.CreatedBy == userId);

            if (existingTicket == null)
            {
                return NotFound();
            }

            // Update only description
            existingTicket.Description = model.Description;
            existingTicket.UpdatedAt = DateTime.Now;

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

                    var attachment = new Attachment
                    {
                        TicketID = existingTicket.TicketID,
                        FilePath = "/uploads/" + fileName,
                        UploadedAt = DateTime.Now
                    };

                    _context.TicketAttachments.Add(attachment);
                }
            }

            _context.SaveChanges();

            return Ok(new
            {
                message = "Ticket updated successfully.",
                ticketId = existingTicket.TicketID
            });
        }





    }
}
