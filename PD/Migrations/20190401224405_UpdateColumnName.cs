using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class UpdateColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SucccessorId",
                table: "PositionAssignments",
                newName: "SuccessorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SuccessorId",
                table: "PositionAssignments",
                newName: "SucccessorId");
        }
    }
}
