namespace QontrolSystem.Models.ViewModels
{
    public class TechnicianTicket
    {
        public int TicketID { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Urgency { get; set; }
        public string CreatedAt { get; set; }
        public int DaysOpen { get; set; }
    }
}
