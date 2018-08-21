using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Data.Migrations
{
    public partial class UpdatedPersonPositionAssociationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_PersonPositions_CurrentPersonId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_CurrentPersonId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "CurrentPersonId",
                table: "Positions");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "PersonPositions",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PersonPositions",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTime>(
                name: "EffectiveDate",
                table: "PersonPositions",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "PersonPositions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonPositions_PositionId",
                table: "PersonPositions",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonPositions_Positions_PositionId",
                table: "PersonPositions",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonPositions_Positions_PositionId",
                table: "PersonPositions");

            migrationBuilder.DropIndex(
                name: "IX_PersonPositions_PositionId",
                table: "PersonPositions");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "PersonPositions");

            migrationBuilder.AddColumn<int>(
                name: "CurrentPersonId",
                table: "Positions",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "PersonPositions",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "PersonPositions",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EffectiveDate",
                table: "PersonPositions",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CurrentPersonId",
                table: "Positions",
                column: "CurrentPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_PersonPositions_CurrentPersonId",
                table: "Positions",
                column: "CurrentPersonId",
                principalTable: "PersonPositions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
