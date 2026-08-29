using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_1qaz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Points",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Points_DepartmentId",
                table: "Points",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Points_TblDepartments_DepartmentId",
                table: "Points",
                column: "DepartmentId",
                principalTable: "TblDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Points_TblDepartments_DepartmentId",
                table: "Points");

            migrationBuilder.DropIndex(
                name: "IX_Points_DepartmentId",
                table: "Points");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Points");
        }
    }
}
