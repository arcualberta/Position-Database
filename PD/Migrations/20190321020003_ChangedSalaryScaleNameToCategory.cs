using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class ChangedSalaryScaleNameToCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "SalaryScales",
                newName: "Category");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "SalaryScales",
                newName: "Name");
        }
    }
}
