using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Ticket
{
    public class History 
    {
        public int HistoryID { get; set; }
        public int TicketID { get; set; }
        public string Action { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; } 
        public string PerformedBy { get; set; } = string.Empty;
        public string? Details { get; set; } = null;
        public Tickets Ticket { get; set; } = null!;
        

    }
}
