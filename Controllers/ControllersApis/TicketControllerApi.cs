using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Controllers.ControllersApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketControllerApi : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketControllerApi(AppDbContext context)
        {
            _context = context;
        }


        //Endpoint to create a new ticket
        [HttpPost]
        [Route("ticket/create")]
        [Authorize(Roles = "Employee")]
        public IActionResult Create([FromForm] CreateTicketViewModel model)
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

            var ticket = new Ticket
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

            return Ok(new
            {
                returnUrl = Url.Action("Tickets", "Ticket"),
                duration = 3000,
                message = "Creating ticket",
            });
        }



    }
}
