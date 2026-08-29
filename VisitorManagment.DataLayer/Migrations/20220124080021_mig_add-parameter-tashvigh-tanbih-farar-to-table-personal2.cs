using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addparametertashvightanbihfarartotablepersonal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FararCount",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LocationJob",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "MarridTitle",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "NahastCount",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TanbihatCount",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "TashvighatCount",
                table: "Files");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "FararCount",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationJob",
                table: "Files",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarridTitle",
                table: "Files",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NahastCount",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TanbihatCount",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TashvighatCount",
                table: "Files",
                type: "int",
                nullable: true);
        }
    }
}
