using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addtablelbkattachmenttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachment_LBKFiles_LBKFileId",
                table: "LBKAttachment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LBKAttachment",
                table: "LBKAttachment");

            migrationBuilder.RenameTable(
                name: "LBKAttachment",
                newName: "LBKAttachments");

            migrationBuilder.RenameIndex(
                name: "IX_LBKAttachment_LBKFileId",
                table: "LBKAttachments",
                newName: "IX_LBKAttachments_LBKFileId");

            migrationBuilder.AddColumn<int>(
                name: "LBKAttachmentTypeId",
                table: "LBKAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LBKAttachments",
                table: "LBKAttachments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LBKAttachmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKAttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LBKAttachments_LBKAttachmentTypeId",
                table: "LBKAttachments",
                column: "LBKAttachmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                table: "LBKAttachments",
                column: "LBKAttachmentTypeId",
                principalTable: "LBKAttachmentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachments_LBKFiles_LBKFileId",
                table: "LBKAttachments",
                column: "LBKFileId",
                principalTable: "LBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                table: "LBKAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachments_LBKFiles_LBKFileId",
                table: "LBKAttachments");

            migrationBuilder.DropTable(
                name: "LBKAttachmentTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LBKAttachments",
                table: "LBKAttachments");

            migrationBuilder.DropIndex(
                name: "IX_LBKAttachments_LBKAttachmentTypeId",
                table: "LBKAttachments");

            migrationBuilder.DropColumn(
                name: "LBKAttachmentTypeId",
                table: "LBKAttachments");

            migrationBuilder.RenameTable(
                name: "LBKAttachments",
                newName: "LBKAttachment");

            migrationBuilder.RenameIndex(
                name: "IX_LBKAttachments_LBKFileId",
                table: "LBKAttachment",
                newName: "IX_LBKAttachment_LBKFileId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LBKAttachment",
                table: "LBKAttachment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachment_LBKFiles_LBKFileId",
                table: "LBKAttachment",
                column: "LBKFileId",
                principalTable: "LBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
