using QontrolSystem.Models.Ticket;
namespace QontrolSystem.Models.Ticket
{
    public class Attachment
    {
        public int TicketAttachmentID { get; set; }
        public int TicketID { get; set; }
        public string FilePath { get; set; } = string.Empty;   
        public DateTime? UploadedAt { get; set; }

        public Tickets Ticket { get; set; }
    }
}
