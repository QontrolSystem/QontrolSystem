﻿using System.ComponentModel.DataAnnotations;

namespace QontrolSystem.Models.Accounts
{
    public class Login
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
