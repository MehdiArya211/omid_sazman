using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_editkbl1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileTypes_FileTypeId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "LBKAttachments");

            migrationBuilder.DropTable(
                name: "LBKCartables");

            migrationBuilder.DropTable(
                name: "LBKHameshs");

            migrationBuilder.DropTable(
                name: "LBKWorkFlows");

            migrationBuilder.DropTable(
                name: "LBKAttachmentTypes");

            migrationBuilder.DropTable(
                name: "LBKFiles");

            migrationBuilder.AlterColumn<int>(
                name: "FileTypeId",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileTypes_FileTypeId",
                table: "Files",
                column: "FileTypeId",
                principalTable: "FileTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_FileTypes_FileTypeId",
                table: "Files");

            migrationBuilder.AlterColumn<int>(
                name: "FileTypeId",
                table: "Files",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "LBKAttachmentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKAttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LBKFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTypeId = table.Column<int>(type: "int", nullable: true),
                    Addres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ArchivedRegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArchivedRegUserId = table.Column<int>(type: "int", nullable: false),
                    CodGha = table.Column<int>(type: "int", maxLength: 200, nullable: false),
                    CodGhaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CountVam = table.Column<int>(type: "int", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: true),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    FarmandehPersonalCode = table.Column<int>(type: "int", nullable: true),
                    FarmandehPersonalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FileStatusId = table.Column<int>(type: "int", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    IsMoavenatAnswered = table.Column<bool>(type: "bit", nullable: false),
                    PersonalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PersonalId = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ReciveMoney = table.Column<long>(type: "bigint", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    RequestDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    RequestSubjectId = table.Column<int>(type: "int", nullable: false),
                    SumAghsatVamMahiyaneh = table.Column<long>(type: "bigint", nullable: false),
                    TotalMoney = table.Column<long>(type: "bigint", nullable: false),
                    UnitCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKFiles_ActionTypes_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "ActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LBKFiles_FileStatuses_FileStatusId",
                        column: x => x.FileStatusId,
                        principalTable: "FileStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LBKFiles_Personals_PersonalId",
                        column: x => x.PersonalId,
                        principalTable: "Personals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LBKFiles_Priorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "Priorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LBKFiles_RequestSubjects_RequestSubjectId",
                        column: x => x.RequestSubjectId,
                        principalTable: "RequestSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKWorkFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    RcvrRoleId = table.Column<int>(type: "int", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    SndrRoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKWorkFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKWorkFlows_Roles_RcvrRoleId",
                        column: x => x.RcvrRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LBKAttachmentTypeId = table.Column<int>(type: "int", nullable: false),
                    LBKFileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKAttachments_LBKAttachmentTypes_LBKAttachmentTypeId",
                        column: x => x.LBKAttachmentTypeId,
                        principalTable: "LBKAttachmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LBKAttachments_LBKFiles_LBKFileId",
                        column: x => x.LBKFileId,
                        principalTable: "LBKFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKCartables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    IsView = table.Column<bool>(type: "bit", nullable: false),
                    LBKFileId = table.Column<int>(type: "int", nullable: false),
                    RcvrUserId = table.Column<int>(type: "int", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SndrUserId = table.Column<int>(type: "int", nullable: false),
                    StateCd = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKCartables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKCartables_LBKFiles_LBKFileId",
                        column: x => x.LBKFileId,
                        principalTable: "LBKFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKHameshs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTypeId = table.Column<int>(type: "int", nullable: false),
                    LBKFileId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RoleTypeId = table.Column<int>(type: "int", nullable: false),
                    RoleTypeTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UserDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKHameshs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKHameshs_ActionTypes_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "ActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LBKHameshs_LBKFiles_LBKFileId",
                        column: x => x.LBKFileId,
                        principalTable: "LBKFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LBKHameshs_LBKHameshs_ParentId",
                        column: x => x.ParentId,
                        principalTable: "LBKHameshs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LBKHameshs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LBKAttachments_LBKAttachmentTypeId",
                table: "LBKAttachments",
                column: "LBKAttachmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKAttachments_LBKFileId",
                table: "LBKAttachments",
                column: "LBKFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKCartables_LBKFileId",
                table: "LBKCartables",
                column: "LBKFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKFiles_ActionTypeId",
                table: "LBKFiles",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKFiles_FileStatusId",
                table: "LBKFiles",
                column: "FileStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKFiles_PersonalId",
                table: "LBKFiles",
                column: "PersonalId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKFiles_PriorityId",
                table: "LBKFiles",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKFiles_RequestSubjectId",
                table: "LBKFiles",
                column: "RequestSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKHameshs_ActionTypeId",
                table: "LBKHameshs",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKHameshs_LBKFileId",
                table: "LBKHameshs",
                column: "LBKFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKHameshs_ParentId",
                table: "LBKHameshs",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKHameshs_UserId",
                table: "LBKHameshs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKWorkFlows_RcvrRoleId",
                table: "LBKWorkFlows",
                column: "RcvrRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_FileTypes_FileTypeId",
                table: "Files",
                column: "FileTypeId",
                principalTable: "FileTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
