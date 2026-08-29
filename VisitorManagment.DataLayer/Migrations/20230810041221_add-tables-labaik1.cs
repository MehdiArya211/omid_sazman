using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addtableslabaik1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachment_lBKFiles_LBKFileId",
                table: "LBKAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKCartables_lBKFiles_LBKFileId",
                table: "LBKCartables");

            migrationBuilder.DropForeignKey(
                name: "FK_lBKFiles_ActionTypes_ActionTypeId",
                table: "lBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_lBKFiles_FileStatuses_FileStatusId",
                table: "lBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_lBKFiles_Personals_PersonalId",
                table: "lBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_lBKFiles_Priorities_PriorityId",
                table: "lBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_lBKFiles_RequestSubjects_RequestSubjectId",
                table: "lBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKHameshs_lBKFiles_LBKFileId",
                table: "LBKHameshs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_lBKFiles",
                table: "lBKFiles");

            migrationBuilder.RenameTable(
                name: "lBKFiles",
                newName: "LBKFiles");

            migrationBuilder.RenameIndex(
                name: "IX_lBKFiles_RequestSubjectId",
                table: "LBKFiles",
                newName: "IX_LBKFiles_RequestSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_lBKFiles_PriorityId",
                table: "LBKFiles",
                newName: "IX_LBKFiles_PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_lBKFiles_PersonalId",
                table: "LBKFiles",
                newName: "IX_LBKFiles_PersonalId");

            migrationBuilder.RenameIndex(
                name: "IX_lBKFiles_FileStatusId",
                table: "LBKFiles",
                newName: "IX_LBKFiles_FileStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_lBKFiles_ActionTypeId",
                table: "LBKFiles",
                newName: "IX_LBKFiles_ActionTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LBKFiles",
                table: "LBKFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachment_LBKFiles_LBKFileId",
                table: "LBKAttachment",
                column: "LBKFileId",
                principalTable: "LBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKCartables_LBKFiles_LBKFileId",
                table: "LBKCartables",
                column: "LBKFileId",
                principalTable: "LBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKFiles_ActionTypes_ActionTypeId",
                table: "LBKFiles",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKFiles_FileStatuses_FileStatusId",
                table: "LBKFiles",
                column: "FileStatusId",
                principalTable: "FileStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKFiles_Personals_PersonalId",
                table: "LBKFiles",
                column: "PersonalId",
                principalTable: "Personals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKFiles_Priorities_PriorityId",
                table: "LBKFiles",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKFiles_RequestSubjects_RequestSubjectId",
                table: "LBKFiles",
                column: "RequestSubjectId",
                principalTable: "RequestSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKHameshs_LBKFiles_LBKFileId",
                table: "LBKHameshs",
                column: "LBKFileId",
                principalTable: "LBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LBKAttachment_LBKFiles_LBKFileId",
                table: "LBKAttachment");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKCartables_LBKFiles_LBKFileId",
                table: "LBKCartables");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKFiles_ActionTypes_ActionTypeId",
                table: "LBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKFiles_FileStatuses_FileStatusId",
                table: "LBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKFiles_Personals_PersonalId",
                table: "LBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKFiles_Priorities_PriorityId",
                table: "LBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKFiles_RequestSubjects_RequestSubjectId",
                table: "LBKFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_LBKHameshs_LBKFiles_LBKFileId",
                table: "LBKHameshs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LBKFiles",
                table: "LBKFiles");

            migrationBuilder.RenameTable(
                name: "LBKFiles",
                newName: "lBKFiles");

            migrationBuilder.RenameIndex(
                name: "IX_LBKFiles_RequestSubjectId",
                table: "lBKFiles",
                newName: "IX_lBKFiles_RequestSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_LBKFiles_PriorityId",
                table: "lBKFiles",
                newName: "IX_lBKFiles_PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_LBKFiles_PersonalId",
                table: "lBKFiles",
                newName: "IX_lBKFiles_PersonalId");

            migrationBuilder.RenameIndex(
                name: "IX_LBKFiles_FileStatusId",
                table: "lBKFiles",
                newName: "IX_lBKFiles_FileStatusId");

            migrationBuilder.RenameIndex(
                name: "IX_LBKFiles_ActionTypeId",
                table: "lBKFiles",
                newName: "IX_lBKFiles_ActionTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_lBKFiles",
                table: "lBKFiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LBKAttachment_lBKFiles_LBKFileId",
                table: "LBKAttachment",
                column: "LBKFileId",
                principalTable: "lBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKCartables_lBKFiles_LBKFileId",
                table: "LBKCartables",
                column: "LBKFileId",
                principalTable: "lBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lBKFiles_ActionTypes_ActionTypeId",
                table: "lBKFiles",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_lBKFiles_FileStatuses_FileStatusId",
                table: "lBKFiles",
                column: "FileStatusId",
                principalTable: "FileStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lBKFiles_Personals_PersonalId",
                table: "lBKFiles",
                column: "PersonalId",
                principalTable: "Personals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lBKFiles_Priorities_PriorityId",
                table: "lBKFiles",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lBKFiles_RequestSubjects_RequestSubjectId",
                table: "lBKFiles",
                column: "RequestSubjectId",
                principalTable: "RequestSubjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LBKHameshs_lBKFiles_LBKFileId",
                table: "LBKHameshs",
                column: "LBKFileId",
                principalTable: "lBKFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
