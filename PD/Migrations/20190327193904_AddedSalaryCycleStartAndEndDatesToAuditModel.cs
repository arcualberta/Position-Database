using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class AddedSalaryCycleStartAndEndDatesToAuditModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryCycle",
                table: "AuditTrail");

            migrationBuilder.AddColumn<DateTime>(
                name: "SalaryCycleEndDate",
                table: "AuditTrail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SalaryCycleStartDate",
                table: "AuditTrail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalaryCycleEndDate",
                table: "AuditTrail");

            migrationBuilder.DropColumn(
                name: "SalaryCycleStartDate",
                table: "AuditTrail");

            migrationBuilder.AddColumn<string>(
                name: "SalaryCycle",
                table: "AuditTrail",
                nullable: true);
        }
    }
}
