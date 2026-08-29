using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class createtablemeeting13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_MeetingStaus_MeetingStausId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_MeetingStausId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "MeetingStausId",
                table: "Meetings");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_MeetingStatusId",
                table: "Meetings",
                column: "MeetingStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_MeetingStaus_MeetingStatusId",
                table: "Meetings",
                column: "MeetingStatusId",
                principalTable: "MeetingStaus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_MeetingStaus_MeetingStatusId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_MeetingStatusId",
                table: "Meetings");

            migrationBuilder.AddColumn<int>(
                name: "MeetingStausId",
                table: "Meetings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_MeetingStausId",
                table: "Meetings",
                column: "MeetingStausId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_MeetingStaus_MeetingStausId",
                table: "Meetings",
                column: "MeetingStausId",
                principalTable: "MeetingStaus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
