using System;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Models;

namespace QontrolSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleID = 1, RoleName = "Employee" },
                new Role { RoleID = 2, RoleName = "Technician" },
                new Role { RoleID = 3, RoleName = "IT Manager" },
                new Role { RoleID = 4, RoleName = "System Administrator" }
            );

            // Seed Departments
            modelBuilder.Entity<Department>().HasData(
                new Department { DepartmentID = 1, DepartmentName = "Incident Handling" },
                new Department { DepartmentID = 2, DepartmentName = "Problem Identification" },
                new Department { DepartmentID = 3, DepartmentName = "Change Management" },
                new Department { DepartmentID = 4, DepartmentName = "Service Request Management" },
                new Department { DepartmentID = 5, DepartmentName = "Other..eg HR,Finance,Operations" }
            );

            
        }
    }
}
