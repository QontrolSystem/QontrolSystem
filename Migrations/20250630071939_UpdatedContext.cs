using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QontrolSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentID",
                keyValue: 3,
                column: "DepartmentName",
                value: "Change Management");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentID",
                keyValue: 4,
                column: "DepartmentName",
                value: "Service Request Management");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentID",
                keyValue: 3,
                column: "DepartmentName",
                value: "Networking");

            migrationBuilder.UpdateData(
                table: "Departments",
                keyColumn: "DepartmentID",
                keyValue: 4,
                column: "DepartmentName",
                value: "Security");
        }
    }
}
