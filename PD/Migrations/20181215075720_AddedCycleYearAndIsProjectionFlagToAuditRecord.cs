using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedCycleYearAndIsProjectionFlagToAuditRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProjectionLog",
                table: "AuditTrail",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SalaryCycle",
                table: "AuditTrail",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProjectionLog",
                table: "AuditTrail");

            migrationBuilder.DropColumn(
                name: "SalaryCycle",
                table: "AuditTrail");
        }
    }
}
