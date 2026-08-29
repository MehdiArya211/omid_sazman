using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addpartVam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VamCodeId",
                table: "Hameshes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VamCode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    SortNum = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VamCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vam",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    VamCodeId = table.Column<int>(type: "int", nullable: false),
                    CodeVam = table.Column<int>(type: "int", nullable: false),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    FilesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vam_Files_FilesId",
                        column: x => x.FilesId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vam_VamCode_VamCodeId",
                        column: x => x.VamCodeId,
                        principalTable: "VamCode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_VamCodeId",
                table: "Hameshes",
                column: "VamCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vam_FilesId",
                table: "Vam",
                column: "FilesId");

            migrationBuilder.CreateIndex(
                name: "IX_Vam_VamCodeId",
                table: "Vam",
                column: "VamCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_VamCode_VamCodeId",
                table: "Hameshes",
                column: "VamCodeId",
                principalTable: "VamCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_VamCode_VamCodeId",
                table: "Hameshes");

            migrationBuilder.DropTable(
                name: "Vam");

            migrationBuilder.DropTable(
                name: "VamCode");

            migrationBuilder.DropIndex(
                name: "IX_Hameshes_VamCodeId",
                table: "Hameshes");

            migrationBuilder.DropColumn(
                name: "VamCodeId",
                table: "Hameshes");
        }
    }
}
