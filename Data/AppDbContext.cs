using System;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Models;
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
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketCategory> TicketCategories { get; set; }
        public DbSet<TicketStatus> TicketStatuses { get; set; }

        public DbSet<TicketUrgency> TicketUrgencies { get; set; }
        public DbSet<TicketFeedback> TicketFeedbacks { get; set; }
        public DbSet<TicketAttachment> TicketAttachments { get; set; } 
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



            
            modelBuilder.Entity<TicketStatus>().HasData(
                new TicketStatus { TicketStatusID = 1, StatusName = "Open" },
                new TicketStatus { TicketStatusID = 2, StatusName = "In Progress" },
                new TicketStatus { TicketStatusID = 3, StatusName = "Resolved" },
                new TicketStatus { TicketStatusID = 4, StatusName = "Closed" }
            );

            
            modelBuilder.Entity<TicketCategory>().HasData(
                new TicketCategory { TicketCategoryID = 1, CategoryName = "Hardware" },
                new TicketCategory { TicketCategoryID = 2, CategoryName = "Software" },
                new TicketCategory { TicketCategoryID = 3, CategoryName = "Network" },
                new TicketCategory { TicketCategoryID = 4, CategoryName = "Security" },
                new TicketCategory { TicketCategoryID = 5, CategoryName = "Other" }
            );

            modelBuilder.Entity<TicketUrgency>().HasData(
                new TicketUrgency { TicketUrgencyID = 1, UrgencyLevel = "Low" },
                new TicketUrgency { TicketUrgencyID = 2, UrgencyLevel = "Medium" },
                new TicketUrgency { TicketUrgencyID = 3, UrgencyLevel = "High" },
                new TicketUrgency { TicketUrgencyID = 4, UrgencyLevel = "Critical" }
            );

            modelBuilder.Entity<TicketFeedback>().HasKey(tf => tf.TicketFeedbackId); 

            // Configure User → Role relationship for ticket creation
            modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Creator)               // navigation property in Ticket
            .WithMany()                           // no back-reference in User
            .HasForeignKey(t => t.CreatedBy)      // FK in Ticket table
            .OnDelete(DeleteBehavior.Restrict);   // don't delete tickets if user is deleted

            // Configure Ticket → User relationship for AssignedTo
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Assignee)            // navigation property in Ticket
                .WithMany()                           // no back-reference in User
                .HasForeignKey(t => t.AssignedTo)     // FK in Ticket table
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
