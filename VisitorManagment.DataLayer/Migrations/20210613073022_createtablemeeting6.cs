using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class createtablemeeting6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_ResponsibleMeetings_ResponsibleMeetingId",
                table: "Meetings");

            migrationBuilder.DropTable(
                name: "ResponsibleMeetings");

            migrationBuilder.DropColumn(
                name: "RegDateMeeting",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "ResponsibleMeetingId",
                table: "Meetings",
                newName: "clerkMeetingId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_ResponsibleMeetingId",
                table: "Meetings",
                newName: "IX_Meetings_clerkMeetingId");

            migrationBuilder.AddColumn<int>(
                name: "BoseMeetingId",
                table: "Meetings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoseMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RankTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoseMeetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClerkMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RankTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClerkMeetings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_BoseMeetingId",
                table: "Meetings",
                column: "BoseMeetingId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_BoseMeetings_BoseMeetingId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_ClerkMeetings_clerkMeetingId",
                table: "Meetings");

            migrationBuilder.DropTable(
                name: "BoseMeetings");

            migrationBuilder.DropTable(
                name: "ClerkMeetings");

            migrationBuilder.DropIndex(
                name: "IX_Meetings_BoseMeetingId",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "BoseMeetingId",
                table: "Meetings");

            migrationBuilder.RenameColumn(
                name: "clerkMeetingId",
                table: "Meetings",
                newName: "ResponsibleMeetingId");

            migrationBuilder.RenameIndex(
                name: "IX_Meetings_clerkMeetingId",
                table: "Meetings",
                newName: "IX_Meetings_ResponsibleMeetingId");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegDateMeeting",
                table: "Meetings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ResponsibleMeetings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    RankTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsibleMeetings", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_ResponsibleMeetings_ResponsibleMeetingId",
                table: "Meetings",
                column: "ResponsibleMeetingId",
                principalTable: "ResponsibleMeetings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
