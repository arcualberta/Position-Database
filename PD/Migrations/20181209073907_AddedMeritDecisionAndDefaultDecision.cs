using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedMeritDecisionAndDefaultDecision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Increment",
                table: "Compensations",
                newName: "MeritDecision");

            migrationBuilder.RenameColumn(
                name: "Change",
                table: "AuditTrail",
                newName: "Message");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultMeritDecision",
                table: "SalaryScales",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AuditType",
                table: "AuditTrail",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultMeritDecision",
                table: "SalaryScales");

            migrationBuilder.DropColumn(
                name: "AuditType",
                table: "AuditTrail");

            migrationBuilder.RenameColumn(
                name: "MeritDecision",
                table: "Compensations",
                newName: "Increment");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "AuditTrail",
                newName: "Change");
        }
    }
}
