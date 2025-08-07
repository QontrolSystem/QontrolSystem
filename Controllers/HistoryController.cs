using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;

namespace QontrolSystem.Controllers
{
    public class HistoryController : Controller
    {
        private readonly AppDbContext _context;

        public HistoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AllTicketHistory()
        {
            var allHistory = await _context.TicketHistories
                .Include(h => h.Ticket)
                .OrderByDescending(h => h.ActionDate)
                .ToListAsync();

            return View(allHistory);
        }

    }
}
