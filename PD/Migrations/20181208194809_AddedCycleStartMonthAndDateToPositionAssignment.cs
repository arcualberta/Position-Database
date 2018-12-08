using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedCycleStartMonthAndDateToPositionAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CycleStartDate",
                table: "PositionAssignments",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CycleStartMonth",
                table: "PositionAssignments",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CycleStartDate",
                table: "PositionAssignments");

            migrationBuilder.DropColumn(
                name: "CycleStartMonth",
                table: "PositionAssignments");
        }
    }
}
