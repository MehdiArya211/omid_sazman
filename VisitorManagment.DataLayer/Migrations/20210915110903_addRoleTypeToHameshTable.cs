using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addRoleTypeToHameshTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Roles",
                newName: "RoleType");

            migrationBuilder.AddColumn<int>(
                name: "RoleType",
                table: "Hameshes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleType",
                table: "Hameshes");

            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "Roles",
                newName: "Type");
        }
    }
}
