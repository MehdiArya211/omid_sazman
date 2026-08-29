using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccesses");

            migrationBuilder.RenameColumn(
                name: "SaveDate",
                table: "Users",
                newName: "RegDate");

            migrationBuilder.RenameColumn(
                name: "AddUserId",
                table: "Users",
                newName: "RegUserId");

            migrationBuilder.RenameColumn(
                name: "SaveDate",
                table: "Personals",
                newName: "RegDate");

            migrationBuilder.RenameColumn(
                name: "AddUserId",
                table: "Personals",
                newName: "RegUserId");

            migrationBuilder.RenameColumn(
                name: "SaveDate",
                table: "Files",
                newName: "RegDate");

            migrationBuilder.RenameColumn(
                name: "AddUserId",
                table: "Files",
                newName: "RegUserId");

            migrationBuilder.CreateTable(
                name: "WorkFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SndrRoleId = table.Column<int>(type: "int", nullable: false),
                    RcvrRoleId = table.Column<int>(type: "int", nullable: false),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkFlows_Roles_RcvrRoleId",
                        column: x => x.RcvrRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlows_RcvrRoleId",
                table: "WorkFlows",
                column: "RcvrRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkFlows");

            migrationBuilder.RenameColumn(
                name: "RegUserId",
                table: "Users",
                newName: "AddUserId");

            migrationBuilder.RenameColumn(
                name: "RegDate",
                table: "Users",
                newName: "SaveDate");

            migrationBuilder.RenameColumn(
                name: "RegUserId",
                table: "Personals",
                newName: "AddUserId");

            migrationBuilder.RenameColumn(
                name: "RegDate",
                table: "Personals",
                newName: "SaveDate");

            migrationBuilder.RenameColumn(
                name: "RegUserId",
                table: "Files",
                newName: "AddUserId");

            migrationBuilder.RenameColumn(
                name: "RegDate",
                table: "Files",
                newName: "SaveDate");

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessRoleId = table.Column<int>(type: "int", nullable: false),
                    AddUserId = table.Column<int>(type: "int", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    SaveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                });
        }
    }
}
