using System.Collections.Generic;
using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Models.ViewModels
{
    public class EmployeeDashboardViewModel
    {
        // Summary stats
        public int TotalTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }

        // Recent tickets
        public List<Tickets> RecentTickets { get; set; }

        // Chart data
        public Dictionary<string, int> TicketStatusCounts { get; set; }
    }
}
