using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Data.Migrations
{
    public partial class CreateAppDataModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    EmployeeId = table.Column<string>(nullable: true),
                    BirthDate = table.Column<DateTime>(nullable: false)
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
                    Guid = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ATBPercentatge = table.Column<decimal>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaryScales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Speedcodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speedcodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChartFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartFields_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonPositions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PersonId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonPositions_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChartStrings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpeedCode = table.Column<string>(nullable: true),
                    ComboCode = table.Column<string>(nullable: true),
                    BusinessUnitId = table.Column<int>(nullable: true),
                    AccountId = table.Column<int>(nullable: true),
                    ClassId = table.Column<int>(nullable: true),
                    DeptIDId = table.Column<int>(nullable: true),
                    FundId = table.Column<int>(nullable: true),
                    ProgramId = table.Column<int>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    SponsorId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartStrings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_AccountId",
                        column: x => x.AccountId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_BusinessUnitId",
                        column: x => x.BusinessUnitId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_ClassId",
                        column: x => x.ClassId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_DeptIDId",
                        column: x => x.DeptIDId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_FundId",
                        column: x => x.FundId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChartStrings_ChartFields_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Number = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    PositionType = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: true),
                    CurrentPersonId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Positions_PersonPositions_CurrentPersonId",
                        column: x => x.CurrentPersonId,
                        principalTable: "PersonPositions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChartField2ChartStringJoin",
                columns: table => new
                {
                    ChartStringId = table.Column<int>(nullable: false),
                    ChartFieldId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChartField2ChartStringJoin", x => new { x.ChartFieldId, x.ChartStringId });
                    table.ForeignKey(
                        name: "FK_ChartField2ChartStringJoin_ChartFields_ChartFieldId",
                        column: x => x.ChartFieldId,
                        principalTable: "ChartFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                        column: x => x.ChartStringId,
                        principalTable: "ChartStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PositionAccount",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChartStringId = table.Column<int>(nullable: false),
                    PositionId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    IsPercentage = table.Column<bool>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    EndDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PositionAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PositionAccount_ChartStrings_ChartStringId",
                        column: x => x.ChartStringId,
                        principalTable: "ChartStrings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PositionAccount_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChartField2ChartStringJoin_ChartStringId",
                table: "ChartField2ChartStringJoin",
                column: "ChartStringId");

            migrationBuilder.CreateIndex(
                name: "IX_ChartFields_DepartmentId",
                table: "ChartFields",
                column: "DepartmentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_PersonPositions_PersonId",
                table: "PersonPositions",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAccount_ChartStringId",
                table: "PositionAccount",
                column: "ChartStringId");

            migrationBuilder.CreateIndex(
                name: "IX_PositionAccount_PositionId",
                table: "PositionAccount",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_CurrentPersonId",
                table: "Positions",
                column: "CurrentPersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChartField2ChartStringJoin");

            migrationBuilder.DropTable(
                name: "PositionAccount");

            migrationBuilder.DropTable(
                name: "SalaryScales");

            migrationBuilder.DropTable(
                name: "Speedcodes");

            migrationBuilder.DropTable(
                name: "ChartStrings");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "ChartFields");

            migrationBuilder.DropTable(
                name: "PersonPositions");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
