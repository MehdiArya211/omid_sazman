using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_editFileTypeTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileType_FileTypeId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileType",
                table: "FileType");

            migrationBuilder.RenameTable(
                name: "FileType",
                newName: "FileTypes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileTypes",
                table: "FileTypes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileTypes_FileTypeId",
                table: "Files",
                column: "FileTypeId",
                principalTable: "FileTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileTypes_FileTypeId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileTypes",
                table: "FileTypes");

            migrationBuilder.RenameTable(
                name: "FileTypes",
                newName: "FileType");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileType",
                table: "FileType",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileType_FileTypeId",
                table: "Files",
                column: "FileTypeId",
                principalTable: "FileType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
