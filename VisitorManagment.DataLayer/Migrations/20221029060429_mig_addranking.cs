using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addranking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vams_Files_FilesId",
                table: "Vams");

            migrationBuilder.DropIndex(
                name: "IX_Vams_FilesId",
                table: "Vams");

            migrationBuilder.DropColumn(
                name: "FilesId",
                table: "Vams");

            migrationBuilder.CreateTable(
                name: "EshrafPeriodDefs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EshrafPeriodDefs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZaribRankings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    Zarib = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZaribRankings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EshrafPeriodDefId = table.Column<int>(type: "int", nullable: false),
                    UnitCode = table.Column<int>(type: "int", nullable: false),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodeGha = table.Column<int>(type: "int", nullable: false),
                    GhaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PointEghdam = table.Column<int>(type: "int", nullable: true),
                    PointReject = table.Column<int>(type: "int", nullable: true),
                    PointNezaja = table.Column<int>(type: "int", nullable: true),
                    FinalPoint = table.Column<int>(type: "int", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Points_EshrafPeriodDefs_EshrafPeriodDefId",
                        column: x => x.EshrafPeriodDefId,
                        principalTable: "EshrafPeriodDefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vams_FileId",
                table: "Vams",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_Points_EshrafPeriodDefId",
                table: "Points",
                column: "EshrafPeriodDefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vams_Files_FileId",
                table: "Vams",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vams_Files_FileId",
                table: "Vams");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "ZaribRankings");

            migrationBuilder.DropTable(
                name: "EshrafPeriodDefs");

            migrationBuilder.DropIndex(
                name: "IX_Vams_FileId",
                table: "Vams");

            migrationBuilder.AddColumn<int>(
                name: "FilesId",
                table: "Vams",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vams_FilesId",
                table: "Vams",
                column: "FilesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vams_Files_FilesId",
                table: "Vams",
                column: "FilesId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
