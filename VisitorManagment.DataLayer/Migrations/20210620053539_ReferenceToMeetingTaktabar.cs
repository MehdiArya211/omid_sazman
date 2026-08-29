using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class ReferenceToMeetingTaktabar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOk",
                table: "Meetings");

            migrationBuilder.AddColumn<bool>(
                name: "IsOkay",
                table: "Meetings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsOkayDate",
                table: "Meetings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IsOkayRegUserId",
                table: "Meetings",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOkay",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "IsOkayDate",
                table: "Meetings");

            migrationBuilder.DropColumn(
                name: "IsOkayRegUserId",
                table: "Meetings");

            migrationBuilder.AddColumn<bool>(
                name: "IsOk",
                table: "Meetings",
                type: "bit",
                nullable: true);
        }
    }
}
