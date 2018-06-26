using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PD.Migrations
{
    public partial class AddedSpeedCodeAndComboCodeToChartString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComboCode",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpeedCode",
                table: "ChartStrings",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComboCode",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "SpeedCode",
                table: "ChartStrings");
        }
    }
}
