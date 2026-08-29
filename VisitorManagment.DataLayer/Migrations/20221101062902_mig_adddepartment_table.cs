using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_adddepartment_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TblDepartmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SortNum = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDepartmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TblDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentCode = table.Column<int>(type: "int", nullable: false),
                    DepartmentFatherCode = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    isMostaghel = table.Column<bool>(type: "bit", nullable: false),
                    UnitCode = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    isVip = table.Column<bool>(type: "bit", nullable: false),
                    DepartmentType = table.Column<int>(type: "int", nullable: false),
                    DepartmentName2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TblDepartmentTypeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TblDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TblDepartments_TblDepartmentTypes_TblDepartmentTypeId",
                        column: x => x.TblDepartmentTypeId,
                        principalTable: "TblDepartmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TblDepartments_TblDepartmentTypeId",
                table: "TblDepartments",
                column: "TblDepartmentTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TblDepartments");

            migrationBuilder.DropTable(
                name: "TblDepartmentTypes");
        }
    }
}
