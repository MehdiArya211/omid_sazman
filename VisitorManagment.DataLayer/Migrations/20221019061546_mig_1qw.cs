using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_1qw : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MablaghVamDarkhasti",
                table: "Files",
                newName: "SumMablaghVamDarkhasti");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SumMablaghVamDarkhasti",
                table: "Files",
                newName: "MablaghVamDarkhasti");
        }
    }
}
