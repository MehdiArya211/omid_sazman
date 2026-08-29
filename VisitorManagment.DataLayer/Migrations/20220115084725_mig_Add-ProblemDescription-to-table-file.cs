using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_AddProblemDescriptiontotablefile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CodGha",
                table: "Files",
                type: "int",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "ProblemDescription",
                table: "Files",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProblemDescription",
                table: "Files");

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
    }
}
