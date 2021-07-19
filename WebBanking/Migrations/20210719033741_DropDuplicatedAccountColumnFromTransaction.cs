using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanking.Migrations
{
    public partial class DropDuplicatedAccountColumnFromTransaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Account_AccountNumber1",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_AccountNumber1",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "AccountNumber1",
                table: "Transaction");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountNumber",
                table: "Transaction",
                column: "AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Account_AccountNumber",
                table: "Transaction",
                column: "AccountNumber",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Account_AccountNumber",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_AccountNumber",
                table: "Transaction");

            migrationBuilder.AddColumn<int>(
                name: "AccountNumber1",
                table: "Transaction",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_AccountNumber1",
                table: "Transaction",
                column: "AccountNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Account_AccountNumber1",
                table: "Transaction",
                column: "AccountNumber1",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
