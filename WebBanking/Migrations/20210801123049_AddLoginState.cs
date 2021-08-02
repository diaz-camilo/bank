using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanking.Migrations
{
    public partial class AddLoginState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Access",
                table: "Login",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Access",
                table: "Login");
        }
    }
}
