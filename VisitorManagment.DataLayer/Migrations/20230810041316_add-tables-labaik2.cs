using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addtableslabaik2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_lBKWorkFlows_Roles_RcvrRoleId",
                table: "lBKWorkFlows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lBKWorkFlows",
                table: "lBKWorkFlows");

            migrationBuilder.RenameTable(
                name: "lBKWorkFlows",
                newName: "LBKWorkFlows");

            migrationBuilder.RenameIndex(
                name: "IX_lBKWorkFlows_RcvrRoleId",
                table: "LBKWorkFlows",
                newName: "IX_LBKWorkFlows_RcvrRoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LBKWorkFlows",
                table: "LBKWorkFlows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LBKWorkFlows_Roles_RcvrRoleId",
                table: "LBKWorkFlows",
                column: "RcvrRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKWorkFlows_Roles_RcvrRoleId",
                table: "LBKWorkFlows");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LBKWorkFlows",
                table: "LBKWorkFlows");

            migrationBuilder.RenameTable(
                name: "LBKWorkFlows",
                newName: "lBKWorkFlows");

            migrationBuilder.RenameIndex(
                name: "IX_LBKWorkFlows_RcvrRoleId",
                table: "lBKWorkFlows",
                newName: "IX_lBKWorkFlows_RcvrRoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lBKWorkFlows",
                table: "lBKWorkFlows",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_lBKWorkFlows_Roles_RcvrRoleId",
                table: "lBKWorkFlows",
                column: "RcvrRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
