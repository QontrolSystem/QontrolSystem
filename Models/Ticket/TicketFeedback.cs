using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Ticket
{
    public class TicketFeedback
    {

        [Key]
        public int TicketFeedbackId { get; set; }
        public int TicketId { get; set; }
        public string Comments { get; set; } = string.Empty;
        public int Rating { get; set; } 
        public DateTime SubmittedAt { get; set; } = DateTime.Now;


        public Ticket Ticket { get; set; }    
    }
}
