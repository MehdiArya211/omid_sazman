using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Users_UsersId",
                table: "UserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserAccesses_UsersId",
                table: "UserAccesses");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "UserAccesses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsersId",
                table: "UserAccesses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UsersId",
                table: "UserAccesses",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Users_UsersId",
                table: "UserAccesses",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
