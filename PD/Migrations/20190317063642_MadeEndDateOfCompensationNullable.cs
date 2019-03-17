using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class MadeEndDateOfCompensationNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ManualOverride",
                table: "Compensations",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(19, 5)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Compensations",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ManualOverride",
                table: "Compensations",
                type: "decimal(19, 5)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Compensations",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
