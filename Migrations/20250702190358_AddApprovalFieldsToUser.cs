using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QontrolSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Users");
        }
    }
}
