using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addparametertashvightanbihfarartotablepersonal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FararCount",
                table: "Personals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationJob",
                table: "Personals",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NahastCount",
                table: "Personals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TanbihatCount",
                table: "Personals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TashvighatCount",
                table: "Personals",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarridTitle",
                table: "Files",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FararCount",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "LocationJob",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "NahastCount",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "TanbihatCount",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "TashvighatCount",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "MarridTitle",
                table: "Files");
        }
    }
}
