using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Models.ViewModels
{
    public class TicketStatusManager
    {
        public List<Tickets>? AssignedTickets { get; set; }
        public List<Tickets>? UnassignedTickets { get; set; }
    }
}
