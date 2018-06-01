using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace PD.Data.Migrations
{
    public partial class CreatedBasicDataModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartField_ChartFieldId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartString_ChartStringId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartString",
                table: "ChartString");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartField",
                table: "ChartField");

            migrationBuilder.RenameTable(
                name: "ChartString",
                newName: "ChartStrings");

            migrationBuilder.RenameTable(
                name: "ChartField",
                newName: "ChartFields");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ChartFields",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "ChartFields",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartStrings",
                table: "ChartStrings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartFields",
                table: "ChartFields",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPositions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaryScales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ATBPercentatge = table.Column<decimal>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Guid = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryScales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CurrentPersonId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    PositionType = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_PersonPositions_CurrentPersonId",
                        column: x => x.CurrentPersonId,
                        principalTable: "PersonPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChartStringId = table.Column<int>(nullable: false),
                    PositionId = table.Column<int>(nullable: false),
                    Proportion = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionAccounts_ChartStrings_ChartStringId",
                        column: x => x.ChartStringId,
                        principalTable: "ChartStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionAccounts_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartFields_DepartmentId",
                table: "ChartFields",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAccounts_ChartStringId",
                table: "PositionAccounts",
                column: "ChartStringId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAccounts_PositionId",
                table: "PositionAccounts",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CurrentPersonId",
                table: "Positions",
                column: "CurrentPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartFields_ChartFieldId",
                table: "ChartField2ChartStringJoin",
                column: "ChartFieldId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin",
                column: "ChartStringId",
                principalTable: "ChartStrings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartFields_Departments_DepartmentId",
                table: "ChartFields",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartFields_ChartFieldId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartFields_Departments_DepartmentId",
                table: "ChartFields");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "PositionAccounts");

            migrationBuilder.DropTable(
                name: "SalaryScales");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "PersonPositions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartStrings",
                table: "ChartStrings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartFields",
                table: "ChartFields");

            migrationBuilder.DropIndex(
                name: "IX_ChartFields_DepartmentId",
                table: "ChartFields");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ChartFields");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ChartFields");

            migrationBuilder.RenameTable(
                name: "ChartStrings",
                newName: "ChartString");

            migrationBuilder.RenameTable(
                name: "ChartFields",
                newName: "ChartField");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartString",
                table: "ChartString",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartField",
                table: "ChartField",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartField_ChartFieldId",
                table: "ChartField2ChartStringJoin",
                column: "ChartFieldId",
                principalTable: "ChartField",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartString_ChartStringId",
                table: "ChartField2ChartStringJoin",
                column: "ChartStringId",
                principalTable: "ChartString",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
