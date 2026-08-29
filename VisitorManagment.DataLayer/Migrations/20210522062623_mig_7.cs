using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Cartables_CartableId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_CartableId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CartableId",
                table: "Files");

            migrationBuilder.CreateIndex(
                name: "IX_Cartables_FileId",
                table: "Cartables",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartables_Files_FileId",
                table: "Cartables",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cartables_Files_FileId",
                table: "Cartables");

            migrationBuilder.DropIndex(
                name: "IX_Cartables_FileId",
                table: "Cartables");

            migrationBuilder.AddColumn<int>(
                name: "CartableId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_CartableId",
                table: "Files",
                column: "CartableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Cartables_CartableId",
                table: "Files",
                column: "CartableId",
                principalTable: "Cartables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
