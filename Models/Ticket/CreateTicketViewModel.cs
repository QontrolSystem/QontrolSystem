namespace QontrolSystem.Models.Ticket
{
    public class CreateTicketViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TicketCategoryID { get; set; }
        public int TicketUrgencyID { get; set; }
        public List<IFormFile> Attachments { get; set; }
    }
}
