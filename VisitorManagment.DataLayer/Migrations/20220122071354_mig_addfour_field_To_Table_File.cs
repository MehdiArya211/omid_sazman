using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addfour_field_To_Table_File : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmploymentDate",
                table: "Files",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FararCount",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationJob",
                table: "Files",
                type: "nvarchar(max)",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentDate",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FararCount",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "LocationJob",
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
    }
}
