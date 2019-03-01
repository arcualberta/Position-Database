using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedSuccessorToPositionAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PositionAssignments_PredecessorId",
                table: "PositionAssignments");

            migrationBuilder.AddColumn<int>(
                name: "SucccessorId",
                table: "PositionAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_PredecessorId",
                table: "PositionAssignments",
                column: "PredecessorId",
                unique: true,
                filter: "[PredecessorId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PositionAssignments_PredecessorId",
                table: "PositionAssignments");

            migrationBuilder.DropColumn(
                name: "SucccessorId",
                table: "PositionAssignments");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_PredecessorId",
                table: "PositionAssignments",
                column: "PredecessorId");
        }
    }
}
