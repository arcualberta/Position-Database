using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PD.Migrations
{
    public partial class UpdatedChartStringModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BusinessUnitId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeptIDId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FundId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SponsorId",
                table: "ChartStrings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_AccountId",
                table: "ChartStrings",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_BusinessUnitId",
                table: "ChartStrings",
                column: "BusinessUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_ClassId",
                table: "ChartStrings",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_DeptIDId",
                table: "ChartStrings",
                column: "DeptIDId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_FundId",
                table: "ChartStrings",
                column: "FundId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_ProgramId",
                table: "ChartStrings",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_ProjectId",
                table: "ChartStrings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartStrings_SponsorId",
                table: "ChartStrings",
                column: "SponsorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_AccountId",
                table: "ChartStrings",
                column: "AccountId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_BusinessUnitId",
                table: "ChartStrings",
                column: "BusinessUnitId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_ClassId",
                table: "ChartStrings",
                column: "ClassId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_DeptIDId",
                table: "ChartStrings",
                column: "DeptIDId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_FundId",
                table: "ChartStrings",
                column: "FundId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_ProgramId",
                table: "ChartStrings",
                column: "ProgramId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_ProjectId",
                table: "ChartStrings",
                column: "ProjectId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_ChartFields_SponsorId",
                table: "ChartStrings",
                column: "SponsorId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_AccountId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_BusinessUnitId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_ClassId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_DeptIDId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_FundId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_ProgramId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_ProjectId",
                table: "ChartStrings");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_ChartFields_SponsorId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_AccountId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_BusinessUnitId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_ClassId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_DeptIDId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_FundId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_ProgramId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_ProjectId",
                table: "ChartStrings");

            migrationBuilder.DropIndex(
                name: "IX_ChartStrings_SponsorId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "BusinessUnitId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "DeptIDId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "FundId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "SponsorId",
                table: "ChartStrings");
        }
    }
}
