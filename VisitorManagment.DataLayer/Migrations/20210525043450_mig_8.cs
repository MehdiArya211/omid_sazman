using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Hameshes_HameshId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hameshes_HameshId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_HameshId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Files_HameshId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "HameshId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HameshId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_ActionTypeId",
                table: "Hameshes",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_FileId",
                table: "Hameshes",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_UserId",
                table: "Hameshes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_ActionTypes_ActionTypeId",
                table: "Hameshes",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_Files_FileId",
                table: "Hameshes",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_Users_UserId",
                table: "Hameshes",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_ActionTypes_ActionTypeId",
                table: "Hameshes");

            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_Files_FileId",
                table: "Hameshes");

            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_Users_UserId",
                table: "Hameshes");

            migrationBuilder.DropIndex(
                name: "IX_Hameshes_ActionTypeId",
                table: "Hameshes");

            migrationBuilder.DropIndex(
                name: "IX_Hameshes_FileId",
                table: "Hameshes");

            migrationBuilder.DropIndex(
                name: "IX_Hameshes_UserId",
                table: "Hameshes");

            migrationBuilder.AddColumn<int>(
                name: "HameshId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HameshId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_HameshId",
                table: "Users",
                column: "HameshId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_HameshId",
                table: "Files",
                column: "HameshId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Hameshes_HameshId",
                table: "Files",
                column: "HameshId",
                principalTable: "Hameshes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Hameshes_HameshId",
                table: "Users",
                column: "HameshId",
                principalTable: "Hameshes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
