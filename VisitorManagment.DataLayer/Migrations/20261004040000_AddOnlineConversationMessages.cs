using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261004040000_AddOnlineConversationMessages")]
    public class AddOnlineConversationMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OnlineConversationMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SenderAvatar = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDelivered = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeliveredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ReadAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlineConversationMessages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OnlineConversationMessages_MeetingId_SentAtUtc",
                table: "OnlineConversationMessages",
                columns: new[] { "MeetingId", "SentAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_OnlineConversationMessages_SenderUserId",
                table: "OnlineConversationMessages",
                column: "SenderUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "OnlineConversationMessages");
        }
    }
}
