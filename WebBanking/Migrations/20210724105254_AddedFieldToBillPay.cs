using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanking.Migrations
{
    public partial class AddedFieldToBillPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "BillPay",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "BillPay");
        }
    }
}
