using Microsoft.EntityFrameworkCore.Migrations;

namespace Riddles.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRiddleStatus_Riddles_FK_RiddleID",
                table: "UserRiddleStatus");

            migrationBuilder.DropIndex(
                name: "IX_UserRiddleStatus_FK_RiddleID",
                table: "UserRiddleStatus");

            migrationBuilder.DropColumn(
                name: "FK_RiddleID",
                table: "UserRiddleStatus");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRiddleStatus_Riddles_RiddleID",
                table: "UserRiddleStatus",
                column: "RiddleID",
                principalTable: "Riddles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRiddleStatus_Riddles_RiddleID",
                table: "UserRiddleStatus");

            migrationBuilder.AddColumn<int>(
                name: "FK_RiddleID",
                table: "UserRiddleStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserRiddleStatus_FK_RiddleID",
                table: "UserRiddleStatus",
                column: "FK_RiddleID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRiddleStatus_Riddles_FK_RiddleID",
                table: "UserRiddleStatus",
                column: "FK_RiddleID",
                principalTable: "Riddles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
