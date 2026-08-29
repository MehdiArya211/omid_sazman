using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class AddTableHameshandActionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionTypeId",
                table: "WorkFlows",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hameshes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ActionTypeId = table.Column<int>(type: "int", nullable: false),
                    UserDesc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hameshes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hameshes_Hameshes_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Hameshes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlows_ActionTypeId",
                table: "WorkFlows",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_HameshId",
                table: "Users",
                column: "HameshId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_HameshId",
                table: "Files",
                column: "HameshId");

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_ParentId",
                table: "Hameshes",
                column: "ParentId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_WorkFlows_ActionTypes_ActionTypeId",
                table: "WorkFlows",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Hameshes_HameshId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Hameshes_HameshId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkFlows_ActionTypes_ActionTypeId",
                table: "WorkFlows");

            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropTable(
                name: "Hameshes");

            migrationBuilder.DropIndex(
                name: "IX_WorkFlows_ActionTypeId",
                table: "WorkFlows");

            migrationBuilder.DropIndex(
                name: "IX_Users_HameshId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Files_HameshId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ActionTypeId",
                table: "WorkFlows");

            migrationBuilder.DropColumn(
                name: "HameshId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HameshId",
                table: "Files");
        }
    }
}
