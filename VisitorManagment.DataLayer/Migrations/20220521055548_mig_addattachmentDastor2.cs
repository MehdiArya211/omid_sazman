using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_addattachmentDastor2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUplodeAttacment",
                table: "FileAttachments");

            migrationBuilder.AddColumn<string>(
                name: "FileUplodeAttacmentDastor",
                table: "FileAttachments",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUplodeAttacmentDastor",
                table: "FileAttachments");

            migrationBuilder.AddColumn<string>(
                name: "FileUplodeAttacment",
                table: "FileAttachments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
