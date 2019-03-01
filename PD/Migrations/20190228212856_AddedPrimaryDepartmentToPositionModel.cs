using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedPrimaryDepartmentToPositionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PrimaryDepartmentId",
                table: "Positions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PrimaryDepartmentId",
                table: "Positions",
                column: "PrimaryDepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Departments_PrimaryDepartmentId",
                table: "Positions",
                column: "PrimaryDepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Departments_PrimaryDepartmentId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_PrimaryDepartmentId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "PrimaryDepartmentId",
                table: "Positions");
        }
    }
}
