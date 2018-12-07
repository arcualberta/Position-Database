using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class RemovedContractSettlementModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Percentage",
                table: "Compensations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Percentage",
                table: "Compensations",
                nullable: true);
        }
    }
}
