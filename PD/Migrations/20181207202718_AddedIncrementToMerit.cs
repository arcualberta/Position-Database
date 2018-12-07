using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedIncrementToMerit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Increment",
                table: "Compensations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Increment",
                table: "Compensations");
        }
    }
}
