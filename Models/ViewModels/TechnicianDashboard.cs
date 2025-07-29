using System.Collections.Generic;

namespace QontrolSystem.Models.ViewModels
{
    public class TechnicianDashboard
    {
        public int TotalAssignedTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int OverdueTickets { get; set; }

        public List<TechnicianTicket> AssignedTickets { get; set; }
    }

}
