using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_updatepersonaltable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BirthDate",
                table: "Personals",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthPlaceTitle",
                table: "Personals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BloodTitle",
                table: "Personals",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmploymentDate",
                table: "Personals",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmploymentTitle",
                table: "Personals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarridTitle",
                table: "Personals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReligoinTitle",
                table: "Personals",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CodGha",
                table: "Files",
                type: "int",
                maxLength: 200,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 200,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "BirthPlaceTitle",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "BloodTitle",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "EmploymentDate",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "EmploymentTitle",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "MarridTitle",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "ReligoinTitle",
                table: "Personals");

            migrationBuilder.AlterColumn<int>(
                name: "CodGha",
                table: "Files",
                type: "int",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 200);
        }
    }
}
