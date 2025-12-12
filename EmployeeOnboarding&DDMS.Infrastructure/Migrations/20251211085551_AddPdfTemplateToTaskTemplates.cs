using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeOnboarding_DDMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPdfTemplateToTaskTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfTemplateFileName",
                table: "TaskTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfTemplateFilePath",
                table: "TaskTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PdfTemplateFileSize",
                table: "TaskTemplates",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PdfTemplateUrl",
                table: "TaskTemplates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfTemplateFileName",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "PdfTemplateFilePath",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "PdfTemplateFileSize",
                table: "TaskTemplates");

            migrationBuilder.DropColumn(
                name: "PdfTemplateUrl",
                table: "TaskTemplates");
        }
    }
}
