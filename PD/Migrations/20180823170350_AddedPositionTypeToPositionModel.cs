using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedPositionTypeToPositionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PositionType",
                table: "Positions",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionType",
                table: "Positions");
        }
    }
}
