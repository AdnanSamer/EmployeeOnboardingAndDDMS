using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeOnboarding_DDMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeePortalFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionUrl",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Severity",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentDocumentId",
                table: "Documents",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionUrl",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ParentDocumentId",
                table: "Documents");
        }
    }
}
