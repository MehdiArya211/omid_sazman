using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addtableslabaik : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "lBKFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionTypeId = table.Column<int>(type: "int", nullable: true),
                    PersonalId = table.Column<int>(type: "int", nullable: false),
                    RequestSubjectId = table.Column<int>(type: "int", nullable: false),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
                    FileStatusId = table.Column<int>(type: "int", nullable: false),
                    RequestDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    PersonalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FarmandehPersonalCode = table.Column<int>(type: "int", nullable: true),
                    FarmandehPersonalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Addres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false),
                    ArchivedRegUserId = table.Column<int>(type: "int", nullable: false),
                    ArchivedRegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    IsMoavenatAnswered = table.Column<bool>(type: "bit", nullable: false),
                    UnitDutyCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitCode = table.Column<int>(type: "int", nullable: true),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGhaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGha = table.Column<int>(type: "int", maxLength: 200, nullable: false),
                    TotalMoney = table.Column<long>(type: "bigint", nullable: false),
                    ReciveMoney = table.Column<long>(type: "bigint", nullable: false),
                    CountVam = table.Column<int>(type: "int", nullable: false),
                    SumAghsatVamMahiyaneh = table.Column<long>(type: "bigint", nullable: false),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    FishAttachment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lBKFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lBKFiles_ActionTypes_ActionTypeId",
                        column: x => x.ActionTypeId,
                        principalTable: "ActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_lBKFiles_FileStatuses_FileStatusId",
                        column: x => x.FileStatusId,
                        principalTable: "FileStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lBKFiles_Personals_PersonalId",
                        column: x => x.PersonalId,
                        principalTable: "Personals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lBKFiles_Priorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "Priorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lBKFiles_RequestSubjects_RequestSubjectId",
                        column: x => x.RequestSubjectId,
                        principalTable: "RequestSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lBKWorkFlows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SndrRoleId = table.Column<int>(type: "int", nullable: false),
                    RcvrRoleId = table.Column<int>(type: "int", nullable: false),
                    RegUserId = table.Column<int>(type: "int", nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lBKWorkFlows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lBKWorkFlows_Roles_RcvrRoleId",
                        column: x => x.RcvrRoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKAttachment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LBKFileId = table.Column<int>(type: "int", nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKAttachment_lBKFiles_LBKFileId",
                        column: x => x.LBKFileId,
                        principalTable: "lBKFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKCartables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RcvrUserId = table.Column<int>(type: "int", nullable: false),
                    SndrUserId = table.Column<int>(type: "int", nullable: false),
                    LBKFileId = table.Column<int>(type: "int", nullable: false),
                    StateCd = table.Column<int>(type: "int", nullable: false),
                    IsView = table.Column<bool>(type: "bit", nullable: false),
                    IsDone = table.Column<bool>(type: "bit", nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LBKCartables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LBKCartables_lBKFiles_LBKFileId",
                        column: x => x.LBKFileId,
                        principalTable: "lBKFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LBKHameshs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LBKFileId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    ActionTypeId = table.Column<int>(type: "int", nullable: false),
                    UserDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleTypeId = table.Column<int>(type: "int", nullable: false),
                    RoleTypeTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RegDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                        name: "FK_LBKHameshs_lBKFiles_LBKFileId",
                        column: x => x.LBKFileId,
                        principalTable: "lBKFiles",
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
                name: "IX_LBKAttachment_LBKFileId",
                table: "LBKAttachment",
                column: "LBKFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LBKCartables_LBKFileId",
                table: "LBKCartables",
                column: "LBKFileId");

            migrationBuilder.CreateIndex(
                name: "IX_lBKFiles_ActionTypeId",
                table: "lBKFiles",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_lBKFiles_FileStatusId",
                table: "lBKFiles",
                column: "FileStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_lBKFiles_PersonalId",
                table: "lBKFiles",
                column: "PersonalId");

            migrationBuilder.CreateIndex(
                name: "IX_lBKFiles_PriorityId",
                table: "lBKFiles",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_lBKFiles_RequestSubjectId",
                table: "lBKFiles",
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
                name: "IX_lBKWorkFlows_RcvrRoleId",
                table: "lBKWorkFlows",
                column: "RcvrRoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LBKAttachment");

            migrationBuilder.DropTable(
                name: "LBKCartables");

            migrationBuilder.DropTable(
                name: "LBKHameshs");

            migrationBuilder.DropTable(
                name: "lBKWorkFlows");

            migrationBuilder.DropTable(
                name: "lBKFiles");


        }
    }
}
