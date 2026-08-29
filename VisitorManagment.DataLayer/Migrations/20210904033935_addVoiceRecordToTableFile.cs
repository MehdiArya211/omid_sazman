using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addVoiceRecordToTableFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VoiceRecord",
                table: "Files",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoiceRecord",
                table: "Files");
        }
    }
}
