using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Users_AccessRoleId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Users_AddUserId",
                table: "UserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserAccesses_AccessRoleId",
                table: "UserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserAccesses_AddUserId",
                table: "UserAccesses");

            migrationBuilder.DropColumn(
                name: "UserUserAccessId",
                table: "UserAccesses");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EditDate",
                table: "Files",
                type: "datetime2",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldMaxLength: 200);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserUserAccessId",
                table: "UserAccesses",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EditDate",
                table: "Files",
                type: "datetime2",
                maxLength: 200,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_AccessRoleId",
                table: "UserAccesses",
                column: "AccessRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_AddUserId",
                table: "UserAccesses",
                column: "AddUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Users_AccessRoleId",
                table: "UserAccesses",
                column: "AccessRoleId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Users_AddUserId",
                table: "UserAccesses",
                column: "AddUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
