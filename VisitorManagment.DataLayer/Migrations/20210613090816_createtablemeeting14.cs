using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class createtablemeeting14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meetings_MeetingPlaceId",
                table: "Meetings");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_MeetingPlaceId",
                table: "Meetings",
                column: "MeetingPlaceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Meetings_MeetingPlaceId",
                table: "Meetings");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_MeetingPlaceId",
                table: "Meetings",
                column: "MeetingPlaceId",
                unique: true);
        }
    }
}
