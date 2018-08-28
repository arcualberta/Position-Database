using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class CreatedFacultyCompensationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Compensations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "ContractSuppliment",
                table: "Compensations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MarketSupplement",
                table: "Compensations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Merit",
                table: "Compensations",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MeritDecision",
                table: "Compensations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeritReason",
                table: "Compensations",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SpecialAdjustment",
                table: "Compensations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Compensations");

            migrationBuilder.DropColumn(
                name: "ContractSuppliment",
                table: "Compensations");

            migrationBuilder.DropColumn(
                name: "MarketSupplement",
                table: "Compensations");

            migrationBuilder.DropColumn(
                name: "Merit",
                table: "Compensations");

            migrationBuilder.DropColumn(
                name: "MeritDecision",
                table: "Compensations");

            migrationBuilder.DropColumn(
                name: "MeritReason",
                table: "Compensations");

            migrationBuilder.DropColumn(
                name: "SpecialAdjustment",
                table: "Compensations");
        }
    }
}
