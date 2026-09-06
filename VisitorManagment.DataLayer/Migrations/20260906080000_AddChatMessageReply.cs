using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20260906080000_AddChatMessageReply")]
    public class AddChatMessageReply : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplyToMessageId",
                table: "ChatMessages",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyToSender",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReplyToMessage",
                table: "ChatMessages",
                type: "nvarchar(180)",
                maxLength: 180,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "ReplyToMessageId", table: "ChatMessages");
            migrationBuilder.DropColumn(name: "ReplyToSender", table: "ChatMessages");
            migrationBuilder.DropColumn(name: "ReplyToMessage", table: "ChatMessages");
        }
    }
}
