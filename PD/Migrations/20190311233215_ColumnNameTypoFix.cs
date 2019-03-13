using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class ColumnNameTypoFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ManuaOverride",
                table: "Compensations",
                newName: "ManualOverride");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ManualOverride",
                table: "Compensations",
                newName: "ManuaOverride");
        }
    }
}
