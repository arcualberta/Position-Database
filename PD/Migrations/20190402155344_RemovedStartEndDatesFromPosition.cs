using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Migrations
{
    public partial class RemovedStartEndDatesFromPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Positions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Positions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Positions",
                nullable: true);
        }
    }
}
