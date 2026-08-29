using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class editlbkfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                table: "LBKAttachments");

            migrationBuilder.DropColumn(
                name: "FishAttachment",
                table: "LBKFiles");

            migrationBuilder.AlterColumn<int>(
                name: "LBKAttachmentTypeId",
                table: "LBKAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                table: "LBKAttachments",
                column: "LBKAttachmentTypeId",
                principalTable: "LBKAttachmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                table: "LBKAttachments");

            migrationBuilder.AddColumn<string>(
                name: "FishAttachment",
                table: "LBKFiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "LBKAttachmentTypeId",
                table: "LBKAttachments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                table: "LBKAttachments",
                column: "LBKAttachmentTypeId",
                principalTable: "LBKAttachmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
