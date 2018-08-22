using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class RenamedValueInPositionAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "PositionAccounts",
                newName: "ValuePercentage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValuePercentage",
                table: "PositionAccounts",
                newName: "Value");
        }
    }
}
