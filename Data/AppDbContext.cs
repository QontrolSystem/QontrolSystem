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
        public DbSet<ITSubDepartment> ITSubDepartments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seed Roles
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


        }
    }
}
