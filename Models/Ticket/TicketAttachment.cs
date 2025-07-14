namespace QontrolSystem.Models.Ticket
{
    public class TicketAttachment
    {
        public int TicketAttachmentID { get; set; }
        public int TicketID { get; set; }
        public string FilePath { get; set; } = string.Empty;   
        public DateTime? UploadedAt { get; set; }

        public Ticket Ticket { get; set; }
    }
}
