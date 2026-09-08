using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20260908050000_AddUnitRoleAccessProfiles")]
    public partial class AddUnitRoleAccessProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitRoleAccessProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitCode = table.Column<int>(type: "int", nullable: false),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitRoleAccessProfiles", x => x.Id);
                    table.ForeignKey("FK_UnitRoleAccessProfiles_Roles_RoleId", x => x.RoleId, "Roles", "RoleId", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitRolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitRoleAccessProfileId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitRolePermissions", x => x.Id);
                    table.ForeignKey("FK_UnitRolePermissions_Permission_PermissionId", x => x.PermissionId, "Permission", "PermissionId", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey("FK_UnitRolePermissions_UnitRoleAccessProfiles_UnitRoleAccessProfileId", x => x.UnitRoleAccessProfileId, "UnitRoleAccessProfiles", "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex("IX_UnitRoleAccessProfiles_RoleId", "UnitRoleAccessProfiles", "RoleId");
            migrationBuilder.CreateIndex("IX_UnitRoleAccessProfiles_UnitCode_RoleId", "UnitRoleAccessProfiles", new[] { "UnitCode", "RoleId" }, unique: true);
            migrationBuilder.CreateIndex("IX_UnitRolePermissions_PermissionId", "UnitRolePermissions", "PermissionId");
            migrationBuilder.CreateIndex("IX_UnitRolePermissions_UnitRoleAccessProfileId_PermissionId", "UnitRolePermissions", new[] { "UnitRoleAccessProfileId", "PermissionId" }, unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UnitRolePermissions");
            migrationBuilder.DropTable(name: "UnitRoleAccessProfiles");
        }
    }
}
