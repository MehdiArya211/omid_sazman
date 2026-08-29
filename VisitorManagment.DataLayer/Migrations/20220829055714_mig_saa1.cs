using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_saa1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MablaghVamDarkhasti",
                table: "Hameshes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MablaghVamMohaghaghSode",
                table: "Hameshes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MablaghVamDarkhasti",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MablaghVamMohaghaghSode",
                table: "Files",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MablaghVamDarkhasti",
                table: "Hameshes");

            migrationBuilder.DropColumn(
                name: "MablaghVamMohaghaghSode",
                table: "Hameshes");

            migrationBuilder.DropColumn(
                name: "MablaghVamDarkhasti",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "MablaghVamMohaghaghSode",
                table: "Files");
        }
    }
}
