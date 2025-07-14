namespace QontrolSystem.Models.Ticket
{
    public class TicketUrgency
    {
        public int TicketUrgencyID { get; set; }
        public string UrgencyLevel { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }
}
