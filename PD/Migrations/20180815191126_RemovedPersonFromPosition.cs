using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PD.Migrations
{
    public partial class RemovedPersonFromPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Persons_PersonId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_PersonId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "Positions");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "PersonPositions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonPositions_PersonId",
                table: "PersonPositions",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonPositions_Persons_PersonId",
                table: "PersonPositions",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonPositions_Persons_PersonId",
                table: "PersonPositions");

            migrationBuilder.DropIndex(
                name: "IX_PersonPositions_PersonId",
                table: "PersonPositions");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "PersonPositions");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "Positions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PersonId",
                table: "Positions",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Persons_PersonId",
                table: "Positions",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
