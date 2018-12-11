using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedPredecessorToPositionAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PredecessorId",
                table: "PositionAssignments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PositionAssignments_PredecessorId",
                table: "PositionAssignments",
                column: "PredecessorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAssignments_PositionAssignments_PredecessorId",
                table: "PositionAssignments",
                column: "PredecessorId",
                principalTable: "PositionAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PositionAssignments_PositionAssignments_PredecessorId",
                table: "PositionAssignments");

            migrationBuilder.DropIndex(
                name: "IX_PositionAssignments_PredecessorId",
                table: "PositionAssignments");

            migrationBuilder.DropColumn(
                name: "PredecessorId",
                table: "PositionAssignments");
        }
    }
}
