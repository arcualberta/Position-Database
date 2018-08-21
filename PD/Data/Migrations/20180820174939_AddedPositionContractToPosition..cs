using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Data.Migrations
{
    public partial class AddedPositionContractToPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionType",
                table: "Positions");

            migrationBuilder.AddColumn<int>(
                name: "PositionContract",
                table: "Positions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PositionWorkload",
                table: "Positions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionContract",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "PositionWorkload",
                table: "Positions");

            migrationBuilder.AddColumn<int>(
                name: "PositionType",
                table: "Positions",
                nullable: false,
                defaultValue: 0);
        }
    }
}
