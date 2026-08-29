using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addrelationhameshwithmeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeetingId",
                table: "Hameshes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_MeetingId",
                table: "Hameshes",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_Meetings_MeetingId",
                table: "Hameshes",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_Meetings_MeetingId",
                table: "Hameshes");

            migrationBuilder.DropIndex(
                name: "IX_Hameshes_MeetingId",
                table: "Hameshes");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "Hameshes");
        }
    }
}
