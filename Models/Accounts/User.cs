namespace QontrolSystem.Models.Accounts
{
    public class User
    {
        public int UserID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string PhoneNumber { get; set; }

        public int RoleID { get; set; }
        public Role Role { get; set; }

        public int DepartmentID { get; set; }
        public Department Department { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool IsRejected { get; set; } = false;

        public int? ITSubDepartmentID { get; set; }
        public ITSubDepartment? ITSubDepartment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
