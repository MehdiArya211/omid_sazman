using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class editrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortNumMoavenat",
                table: "Roles",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortNumMoavenat",
                table: "Roles");
        }
    }
}
