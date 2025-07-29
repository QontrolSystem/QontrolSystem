using System;

namespace QontrolSystem.Models.ViewModels
{
    public class TechnicianProfile
    {
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
