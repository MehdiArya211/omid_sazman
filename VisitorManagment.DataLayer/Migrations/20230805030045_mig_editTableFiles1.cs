using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_editTableFiles1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionTypeId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ActionTypeId",
                table: "Files",
                column: "ActionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_ActionTypes_ActionTypeId",
                table: "Files",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_ActionTypes_ActionTypeId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ActionTypeId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ActionTypeId",
                table: "Files");
        }
    }
}
