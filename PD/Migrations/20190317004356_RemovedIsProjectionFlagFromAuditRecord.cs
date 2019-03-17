using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class RemovedIsProjectionFlagFromAuditRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProjectionLog",
                table: "AuditTrail");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProjectionLog",
                table: "AuditTrail",
                nullable: false,
                defaultValue: false);
        }
    }
}
