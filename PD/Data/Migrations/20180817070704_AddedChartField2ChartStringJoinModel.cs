using Microsoft.EntityFrameworkCore.Migrations;

namespace PD.Data.Migrations
{
    public partial class AddedChartField2ChartStringJoinModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartFields_ChartFieldId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoin_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartField2ChartStringJoin",
                table: "ChartField2ChartStringJoin");

            migrationBuilder.RenameTable(
                name: "ChartField2ChartStringJoin",
                newName: "ChartField2ChartStringJoins");

            migrationBuilder.RenameIndex(
                name: "IX_ChartField2ChartStringJoin_ChartStringId",
                table: "ChartField2ChartStringJoins",
                newName: "IX_ChartField2ChartStringJoins_ChartStringId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartField2ChartStringJoins",
                table: "ChartField2ChartStringJoins",
                columns: new[] { "ChartFieldId", "ChartStringId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoins_ChartFields_ChartFieldId",
                table: "ChartField2ChartStringJoins",
                column: "ChartFieldId",
                principalTable: "ChartFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChartField2ChartStringJoins_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoins",
                column: "ChartStringId",
                principalTable: "ChartStrings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoins_ChartFields_ChartFieldId",
                table: "ChartField2ChartStringJoins");

            migrationBuilder.DropForeignKey(
                name: "FK_ChartField2ChartStringJoins_ChartStrings_ChartStringId",
                table: "ChartField2ChartStringJoins");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChartField2ChartStringJoins",
                table: "ChartField2ChartStringJoins");

            migrationBuilder.RenameTable(
                name: "ChartField2ChartStringJoins",
                newName: "ChartField2ChartStringJoin");

            migrationBuilder.RenameIndex(
                name: "IX_ChartField2ChartStringJoins_ChartStringId",
                table: "ChartField2ChartStringJoin",
                newName: "IX_ChartField2ChartStringJoin_ChartStringId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChartField2ChartStringJoin",
                table: "ChartField2ChartStringJoin",
                columns: new[] { "ChartFieldId", "ChartStringId" });

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
        }
    }
}
