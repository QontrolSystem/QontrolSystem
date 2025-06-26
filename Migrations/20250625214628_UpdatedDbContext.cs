using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QontrolSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentID", "DepartmentName" },
                values: new object[] { 5, "Other..eg HR,Finance,Operations" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "DepartmentID",
                keyValue: 5);
        }
    }
}
