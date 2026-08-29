using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addRoleTypeTitleInTableHamesh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RoleType",
                table: "Hameshes",
                newName: "RoleTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "RoleType",
                table: "Roles",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoleTypeTitle",
                table: "Hameshes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleTypeTitle",
                table: "Hameshes");

            migrationBuilder.RenameColumn(
                name: "RoleTypeId",
                table: "Hameshes",
                newName: "RoleType");

            migrationBuilder.AlterColumn<int>(
                name: "RoleType",
                table: "Roles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
