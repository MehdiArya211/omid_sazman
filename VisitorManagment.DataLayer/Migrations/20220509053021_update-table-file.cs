using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class updatetablefile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FishUplodeAttacment",
                table: "FileAttachments");

            migrationBuilder.AddColumn<string>(
                name: "FishAttachment",
                table: "Files",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FishAttachment",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "FishUplodeAttacment",
                table: "FileAttachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
