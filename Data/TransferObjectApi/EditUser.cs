namespace QontrolSystem.Data.TransferObjectApi
{
    public class EditUser
    {
        public int UserID { get; set; }  
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int RoleID { get; set; }
        public int DepartmentID { get; set; }
        public bool IsActive { get; set; }
        public string? NewPassword { get; set; }
    }
}
