using System;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Models.Accounts;
using QontrolSystem.Models.Ticket;

namespace QontrolSystem.Data
{
    public class AppDbContext : DbContext
    {
        internal object PasswordResetOtp;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<ITSubDepartment> ITSubDepartments { get; set; }

        public DbSet<PasswordResetOtp> PasswordResetOtps { get; set; }
        public DbSet<Tickets> Tickets { get; set; }
        public DbSet<Category> TicketCategories { get; set; }
        public DbSet<Status> TicketStatuses { get; set; }

        public DbSet<Urgency> TicketUrgencies { get; set; }
        public DbSet<Feedback> TicketFeedbacks { get; set; }
        public DbSet<Attachment> TicketAttachments { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, RoleName = "Employee" },
                new Role { RoleID = 2, RoleName = "Technician" },
                new Role { RoleID = 3, RoleName = "IT Manager" },
                new Role { RoleID = 4, RoleName = "System Administrator" }
            );

            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentID = 1, DepartmentName = "HR" },
                new Department { DepartmentID = 2, DepartmentName = "Finance" },
                new Department { DepartmentID = 3, DepartmentName = "Operations" },
                new Department { DepartmentID = 4, DepartmentName = "IT Department" }
            );


            modelBuilder.Entity<ITSubDepartment>().HasData(
                new ITSubDepartment { ITSubDepartmentID = 1, SubDepartmentName = "Software", DepartmentID = 4 },
                new ITSubDepartment { ITSubDepartmentID = 2, SubDepartmentName = "Hardware", DepartmentID = 4 },
                new ITSubDepartment { ITSubDepartmentID = 3, SubDepartmentName = "Network", DepartmentID = 4 },
                new ITSubDepartment { ITSubDepartmentID = 4, SubDepartmentName = "Security", DepartmentID = 4 }
            );



            
            modelBuilder.Entity<Status>().HasData(
                new Status { TicketStatusID = 1, StatusName = "Open" },
                new Status { TicketStatusID = 2, StatusName = "In Progress" },
                new Status { TicketStatusID = 3, StatusName = "Resolved" },
                new Status { TicketStatusID = 4, StatusName = "Closed" }
            );

            
            modelBuilder.Entity<Category>().HasData(
                new Category { TicketCategoryID = 1, CategoryName = "Hardware" },
                new Category { TicketCategoryID = 2, CategoryName = "Software" },
                new Category { TicketCategoryID = 3, CategoryName = "Network" },
                new Category { TicketCategoryID = 4, CategoryName = "Security" },
                new Category { TicketCategoryID = 5, CategoryName = "Other" }
            );

            modelBuilder.Entity<Urgency>().HasData(
                new Urgency { TicketUrgencyID = 1, UrgencyLevel = "Low" },
                new Urgency { TicketUrgencyID = 2, UrgencyLevel = "Medium" },
                new Urgency { TicketUrgencyID = 3, UrgencyLevel = "High" },
                new Urgency { TicketUrgencyID = 4, UrgencyLevel = "Critical" }
            );

            modelBuilder.Entity<Feedback>().HasKey(tf => tf.TicketFeedbackId); 

            // Configure User → Role relationship for ticket creation
            modelBuilder.Entity<Tickets>()
            .HasOne(t => t.Creator)               // navigation property in Ticket
            .WithMany()                           // no back-reference in User
            .HasForeignKey(t => t.CreatedBy)      // FK in Ticket table
            .OnDelete(DeleteBehavior.Restrict);   // don't delete tickets if user is deleted

            // Configure Ticket → User relationship for AssignedTo
            modelBuilder.Entity<Tickets>()
                .HasOne(t => t.Assignee)            // navigation property in Ticket
                .WithMany()                           // no back-reference in User
                .HasForeignKey(t => t.AssignedTo)     // FK in Ticket table
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
