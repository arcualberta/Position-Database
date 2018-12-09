using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class ChangedFtPtStatusToWorkload : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Workload",
                table: "Positions",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Workload",
                table: "Positions",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
