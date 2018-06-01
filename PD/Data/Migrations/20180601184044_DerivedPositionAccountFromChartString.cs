using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PD.Data.Migrations
{
    public partial class DerivedPositionAccountFromChartString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAccounts_ChartStrings_ChartStringId",
                table: "PositionAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_PositionAccounts_Positions_PositionId",
                table: "PositionAccounts");

            migrationBuilder.DropTable(
                name: "ChartStrings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PositionAccounts",
                table: "PositionAccounts");

            migrationBuilder.DropIndex(
                name: "IX_PositionAccounts_ChartStringId",
                table: "PositionAccounts");

            migrationBuilder.DropColumn(
                name: "ChartStringId",
                table: "PositionAccounts");

            migrationBuilder.RenameTable(
                name: "PositionAccounts",
                newName: "ChartStrings");

            migrationBuilder.RenameIndex(
                name: "IX_PositionAccounts_PositionId",
                table: "ChartStrings",
                newName: "IX_ChartStrings_PositionId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Proportion",
                table: "ChartStrings",
                nullable: true,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "ChartStrings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ChartStrings",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartStrings",
                table: "ChartStrings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin",
                column: "ChartStringId",
                principalTable: "ChartStrings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartStrings_Positions_PositionId",
                table: "ChartStrings",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartStrings_Positions_PositionId",
                table: "ChartStrings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartStrings",
                table: "ChartStrings");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ChartStrings");

            migrationBuilder.RenameTable(
                name: "ChartStrings",
                newName: "PositionAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_ChartStrings_PositionId",
                table: "PositionAccounts",
                newName: "IX_PositionAccounts_PositionId");

            migrationBuilder.AlterColumn<decimal>(
                name: "Proportion",
                table: "PositionAccounts",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PositionId",
                table: "PositionAccounts",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChartStringId",
                table: "PositionAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PositionAccounts",
                table: "PositionAccounts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ChartStrings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartStrings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PositionAccounts_ChartStringId",
                table: "PositionAccounts",
                column: "ChartStringId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin",
                column: "ChartStringId",
                principalTable: "ChartStrings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAccounts_ChartStrings_ChartStringId",
                table: "PositionAccounts",
                column: "ChartStringId",
                principalTable: "ChartStrings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PositionAccounts_Positions_PositionId",
                table: "PositionAccounts",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
