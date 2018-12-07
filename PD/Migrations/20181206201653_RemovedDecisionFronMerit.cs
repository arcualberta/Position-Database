using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class RemovedDecisionFronMerit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Decision",
                table: "Compensations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Decision",
                table: "Compensations",
                nullable: true);
        }
    }
}
