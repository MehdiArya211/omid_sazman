using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addfildToHamesh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleTypeFinalId",
                table: "Hameshes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleTypeFinalTitle",
                table: "Hameshes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleTypeFinalId",
                table: "Hameshes");

            migrationBuilder.DropColumn(
                name: "RoleTypeFinalTitle",
                table: "Hameshes");
        }
    }
}
