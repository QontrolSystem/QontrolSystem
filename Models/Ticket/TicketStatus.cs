namespace QontrolSystem.Models.Ticket
{
    public class TicketStatus
    {
        public int StatusID { get; set; }
        public string StatusName { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        // Override ToString() for better readability
        public override string ToString()
        {
            return $"{StatusID}: {StatusName}";
        }
    }
}
