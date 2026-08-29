using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class createtablemeeting11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_BoseMeetings_BoseMeetingId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_ClerkMeetings_clerkMeetingId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "BoseId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "ClerkId",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "clerkMeetingId",
                table: "Meetings",
                newName: "ClerkMeetingId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_clerkMeetingId",
                table: "Meetings",
                newName: "IX_Meetings_ClerkMeetingId");

            migrationBuilder.AlterColumn<int>(
                name: "ClerkMeetingId",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BoseMeetingId",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_BoseMeetings_BoseMeetingId",
                table: "Meetings",
                column: "BoseMeetingId",
                principalTable: "BoseMeetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_ClerkMeetings_ClerkMeetingId",
                table: "Meetings",
                column: "ClerkMeetingId",
                principalTable: "ClerkMeetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_BoseMeetings_BoseMeetingId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_ClerkMeetings_ClerkMeetingId",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "ClerkMeetingId",
                table: "Meetings",
                newName: "clerkMeetingId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_ClerkMeetingId",
                table: "Meetings",
                newName: "IX_Meetings_clerkMeetingId");

            migrationBuilder.AlterColumn<int>(
                name: "clerkMeetingId",
                table: "Meetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BoseMeetingId",
                table: "Meetings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "BoseId",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClerkId",
                table: "Meetings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_BoseMeetings_BoseMeetingId",
                table: "Meetings",
                column: "BoseMeetingId",
                principalTable: "BoseMeetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_ClerkMeetings_clerkMeetingId",
                table: "Meetings",
                column: "clerkMeetingId",
                principalTable: "ClerkMeetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
