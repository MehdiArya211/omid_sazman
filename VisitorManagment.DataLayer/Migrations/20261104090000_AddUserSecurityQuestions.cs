using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261104090000_AddUserSecurityQuestions")]
    public class AddUserSecurityQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSecurityQuestions",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Answer1Hash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Answer2Hash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Answer3Hash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    LockedUntilUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurityQuestions", item => item.UserId);
                    table.CheckConstraint("CK_UserSecurityQuestions_FailedAttempts", "[FailedAttempts] >= 0 AND [FailedAttempts] <= 5");
                    table.ForeignKey(
                        name: "FK_UserSecurityQuestions_Users_UserId",
                        column: item => item.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "UserSecurityQuestions");
        }
    }
}
