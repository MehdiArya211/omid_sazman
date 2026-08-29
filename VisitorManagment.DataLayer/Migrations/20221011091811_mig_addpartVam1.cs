using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addpartVam1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_VamCode_VamCodeId",
                table: "Hameshes");

            migrationBuilder.DropForeignKey(
                name: "FK_Vam_Files_FilesId",
                table: "Vam");

            migrationBuilder.DropForeignKey(
                name: "FK_Vam_VamCode_VamCodeId",
                table: "Vam");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VamCode",
                table: "VamCode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vam",
                table: "Vam");

            migrationBuilder.RenameTable(
                name: "VamCode",
                newName: "VamCodes");

            migrationBuilder.RenameTable(
                name: "Vam",
                newName: "Vams");

            migrationBuilder.RenameIndex(
                name: "IX_Vam_VamCodeId",
                table: "Vams",
                newName: "IX_Vams_VamCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Vam_FilesId",
                table: "Vams",
                newName: "IX_Vams_FilesId");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Vams",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VamCodes",
                table: "VamCodes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vams",
                table: "Vams",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_VamCodes_VamCodeId",
                table: "Hameshes",
                column: "VamCodeId",
                principalTable: "VamCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vams_Files_FilesId",
                table: "Vams",
                column: "FilesId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vams_VamCodes_VamCodeId",
                table: "Vams",
                column: "VamCodeId",
                principalTable: "VamCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_VamCodes_VamCodeId",
                table: "Hameshes");

            migrationBuilder.DropForeignKey(
                name: "FK_Vams_Files_FilesId",
                table: "Vams");

            migrationBuilder.DropForeignKey(
                name: "FK_Vams_VamCodes_VamCodeId",
                table: "Vams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vams",
                table: "Vams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VamCodes",
                table: "VamCodes");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Vams");

            migrationBuilder.RenameTable(
                name: "Vams",
                newName: "Vam");

            migrationBuilder.RenameTable(
                name: "VamCodes",
                newName: "VamCode");

            migrationBuilder.RenameIndex(
                name: "IX_Vams_VamCodeId",
                table: "Vam",
                newName: "IX_Vam_VamCodeId");

            migrationBuilder.RenameIndex(
                name: "IX_Vams_FilesId",
                table: "Vam",
                newName: "IX_Vam_FilesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vam",
                table: "Vam",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VamCode",
                table: "VamCode",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_VamCode_VamCodeId",
                table: "Hameshes",
                column: "VamCodeId",
                principalTable: "VamCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vam_Files_FilesId",
                table: "Vam",
                column: "FilesId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vam_VamCode_VamCodeId",
                table: "Vam",
                column: "VamCodeId",
                principalTable: "VamCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
