using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanking.Migrations
{
    public partial class FixForeignKeyInBillPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPay_Account_AccountNumber1",
                table: "BillPay");

            migrationBuilder.DropIndex(
                name: "IX_BillPay_AccountNumber1",
                table: "BillPay");

            migrationBuilder.DropColumn(
                name: "AccountNumber1",
                table: "BillPay");

            migrationBuilder.CreateIndex(
                name: "IX_BillPay_AccountNumber",
                table: "BillPay",
                column: "AccountNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_BillPay_Account_AccountNumber",
                table: "BillPay",
                column: "AccountNumber",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BillPay_Account_AccountNumber",
                table: "BillPay");

            migrationBuilder.DropIndex(
                name: "IX_BillPay_AccountNumber",
                table: "BillPay");

            migrationBuilder.AddColumn<int>(
                name: "AccountNumber1",
                table: "BillPay",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillPay_AccountNumber1",
                table: "BillPay",
                column: "AccountNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_BillPay_Account_AccountNumber1",
                table: "BillPay",
                column: "AccountNumber1",
                principalTable: "Account",
                principalColumn: "AccountNumber",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
