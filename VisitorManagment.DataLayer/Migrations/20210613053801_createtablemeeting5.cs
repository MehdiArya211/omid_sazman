using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class createtablemeeting5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibleMeetings_Meetings_MeetingId",
                table: "ResponsibleMeetings");

            migrationBuilder.DropIndex(
                name: "IX_ResponsibleMeetings_MeetingId",
                table: "ResponsibleMeetings");

            migrationBuilder.DropColumn(
                name: "MeetingId",
                table: "ResponsibleMeetings");

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleMeetingId",
                table: "Meetings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_ResponsibleMeetingId",
                table: "Meetings",
                column: "ResponsibleMeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_ResponsibleMeetings_ResponsibleMeetingId",
                table: "Meetings",
                column: "ResponsibleMeetingId",
                principalTable: "ResponsibleMeetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_ResponsibleMeetings_ResponsibleMeetingId",
                table: "Meetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_ResponsibleMeetingId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "ResponsibleMeetingId",
                table: "Meetings");

            migrationBuilder.AddColumn<int>(
                name: "MeetingId",
                table: "ResponsibleMeetings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponsibleMeetings_MeetingId",
                table: "ResponsibleMeetings",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibleMeetings_Meetings_MeetingId",
                table: "ResponsibleMeetings",
                column: "MeetingId",
                principalTable: "Meetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
