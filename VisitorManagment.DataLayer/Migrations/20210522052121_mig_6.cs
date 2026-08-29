using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CartableId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cartables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RcvrUserId = table.Column<int>(type: "int", nullable: false),
                    SndrUserId = table.Column<int>(type: "int", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    StateCd = table.Column<int>(type: "int", nullable: false),
                    IsView = table.Column<bool>(type: "bit", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cartables", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Cartables_CartableId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "Cartables");

            migrationBuilder.DropIndex(
                name: "IX_Files_CartableId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "CartableId",
                table: "Files");
        }
    }
}
