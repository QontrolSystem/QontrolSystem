﻿namespace QontrolSystem.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
