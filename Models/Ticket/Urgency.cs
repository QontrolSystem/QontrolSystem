﻿using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Ticket
{
    public class Urgency
    {
        [Key]
        public int TicketUrgencyID { get; set; }
        public string UrgencyLevel { get; set; } = string.Empty;
        // Navigation property for Tickets
        public ICollection<Tickets> Tickets { get; set; } = new List<Tickets>();

    }
}
