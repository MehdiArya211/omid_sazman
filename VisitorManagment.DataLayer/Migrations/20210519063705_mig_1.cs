using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentID = table.Column<int>(type: "int", nullable: true),
                    ParentUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ShowAll = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IconName = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_Permission_Permission_ParentID",
                        column: x => x.ParentID,
                        principalTable: "Permission",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Personals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MelliCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RankTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RankCode = table.Column<int>(type: "int", nullable: true),
                    BranchTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BranchCode = table.Column<int>(type: "int", nullable: true),
                    JobDes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EzamDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatusTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JanbaziArtesh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JanbaziBonyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsarStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AmaliatiKhedmate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhableGhatnameAmaliatiKhedmate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitDutyCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitCode = table.Column<int>(type: "int", nullable: true),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGhaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGha = table.Column<int>(type: "int", nullable: true),
                    FarmandehPersonalCode = table.Column<int>(type: "int", nullable: true),
                    FarmandehPersonalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Addres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddUserId = table.Column<int>(type: "int", nullable: false),
                    SaveDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Priorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Priorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestSubjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BranchCode = table.Column<int>(type: "int", nullable: true),
                    JobDes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ActiveCode = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserAvatar = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RankTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RankCode = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    UnitDutyCode = table.Column<int>(type: "int", nullable: true),
                    UnitCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGhaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGha = table.Column<int>(type: "int", nullable: true),
                    AddUserId = table.Column<int>(type: "int", nullable: false),
                    SaveDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonalId = table.Column<int>(type: "int", nullable: false),
                    RequestSubjectId = table.Column<int>(type: "int", nullable: false),
                    PriorityId = table.Column<int>(type: "int", nullable: false),
                    FileStatusId = table.Column<int>(type: "int", nullable: false),
                    RequestDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PersonalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MelliCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RankTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RankCode = table.Column<int>(type: "int", nullable: true),
                    BranchTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BranchCode = table.Column<int>(type: "int", nullable: true),
                    JobDes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EzamDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatusTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JanbaziArtesh = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JanbaziBonyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsarStatus = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AmaliatiKhedmate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GhableGhatnameAmaliatiKhedmate = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitDutyCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitCode = table.Column<int>(type: "int", nullable: true),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGhaTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodGha = table.Column<int>(type: "int", maxLength: 200, nullable: true),
                    FarmandehPersonalCode = table.Column<int>(type: "int", nullable: true),
                    FarmandehPersonalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Addres = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    AddUserId = table.Column<int>(type: "int", nullable: false),
                    SaveDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_FileStatuses_FileStatusId",
                        column: x => x.FileStatusId,
                        principalTable: "FileStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_Personals_PersonalId",
                        column: x => x.PersonalId,
                        principalTable: "Personals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_Priorities_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "Priorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Files_RequestSubjects_RequestSubjectId",
                        column: x => x.RequestSubjectId,
                        principalTable: "RequestSubjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RP_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => x.RP_Id);
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AccessRoleId = table.Column<int>(type: "int", nullable: false),
                    AddUserId = table.Column<int>(type: "int", nullable: false),
                    EditUserId = table.Column<int>(type: "int", nullable: true),
                    SaveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsersId = table.Column<int>(type: "int", nullable: true),
                    UserUserAccessId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Users_AccessRoleId",
                        column: x => x.AccessRoleId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAccesses_Users_AddUserId",
                        column: x => x.AddUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserAccesses_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UR_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UR_Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileStatusId",
                table: "Files",
                column: "FileStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_PersonalId",
                table: "Files",
                column: "PersonalId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_PriorityId",
                table: "Files",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_RequestSubjectId",
                table: "Files",
                column: "RequestSubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_ParentID",
                table: "Permission",
                column: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_RoleId",
                table: "RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_AccessRoleId",
                table: "UserAccesses",
                column: "AccessRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_AddUserId",
                table: "UserAccesses",
                column: "AddUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UsersId",
                table: "UserAccesses",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "UserAccesses");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "FileStatuses");

            migrationBuilder.DropTable(
                name: "Personals");

            migrationBuilder.DropTable(
                name: "Priorities");

            migrationBuilder.DropTable(
                name: "RequestSubjects");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
