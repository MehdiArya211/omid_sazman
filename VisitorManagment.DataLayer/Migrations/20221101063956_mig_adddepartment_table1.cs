using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_adddepartment_table1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName2",
                table: "TblDepartments");

            migrationBuilder.RenameColumn(
                name: "isActive",
                table: "TblDepartmentTypes",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "isVip",
                table: "TblDepartments",
                newName: "IsVip");

            migrationBuilder.RenameColumn(
                name: "isMostaghel",
                table: "TblDepartments",
                newName: "IsMostaghel");

            migrationBuilder.RenameColumn(
                name: "DepartmentType",
                table: "TblDepartments",
                newName: "DepartmentTypeId");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "TblDepartmentTypes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "TblDepartments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentTitle",
                table: "TblDepartments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentTitle",
                table: "TblDepartments");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "TblDepartmentTypes",
                newName: "isActive");

            migrationBuilder.RenameColumn(
                name: "IsVip",
                table: "TblDepartments",
                newName: "isVip");

            migrationBuilder.RenameColumn(
                name: "IsMostaghel",
                table: "TblDepartments",
                newName: "isMostaghel");

            migrationBuilder.RenameColumn(
                name: "DepartmentTypeId",
                table: "TblDepartments",
                newName: "DepartmentType");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "TblDepartmentTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "TblDepartments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName2",
                table: "TblDepartments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
