using Microsoft.EntityFrameworkCore.Migrations;

namespace WebBanking.Migrations
{
    public partial class RefreshTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "Payee",
                newName: "Phone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Payee",
                newName: "Mobile");
        }
    }
}
