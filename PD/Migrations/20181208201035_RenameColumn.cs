using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class RenameColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CycleStartMonth",
                table: "PositionAssignments",
                newName: "SalaryCycleStartMonth");

            migrationBuilder.RenameColumn(
                name: "CycleStartDate",
                table: "PositionAssignments",
                newName: "SalaryCycleStartDay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SalaryCycleStartMonth",
                table: "PositionAssignments",
                newName: "CycleStartMonth");

            migrationBuilder.RenameColumn(
                name: "SalaryCycleStartDay",
                table: "PositionAssignments",
                newName: "CycleStartDate");
        }
    }
}
