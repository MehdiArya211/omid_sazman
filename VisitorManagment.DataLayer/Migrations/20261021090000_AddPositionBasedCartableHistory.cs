using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261021090000_AddPositionBasedCartableHistory")]
    public partial class AddPositionBasedCartableHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RcvrRoleId",
                table: "Cartables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ActorFullNameSnapshot",
                table: "Hameshes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ActorPersonalCodeSnapshot",
                table: "Hameshes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ActorRankTitleSnapshot",
                table: "Hameshes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "ActorUnitTitleSnapshot",
                table: "Hameshes",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoleAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<System.DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<System.DateTime>(type: "datetime2", nullable: true),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    ChangeReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_RoleAssignmentHistories", item => item.Id));

            migrationBuilder.CreateIndex(
                name: "IX_Cartables_RcvrRoleId",
                table: "Cartables",
                column: "RcvrRoleId");
            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignmentHistories_RoleId",
                table: "RoleAssignmentHistories",
                column: "RoleId",
                unique: true,
                filter: "[EndDate] IS NULL");
            migrationBuilder.CreateIndex(
                name: "IX_RoleAssignmentHistories_UserId",
                table: "RoleAssignmentHistories",
                column: "UserId",
                unique: true,
                filter: "[EndDate] IS NULL");

            // داده‌های قدیمی فقط تکمیل می‌شوند و هیچ رکوردی حذف یا بازنویسی هویتی نمی‌شود.
            migrationBuilder.Sql(@"
UPDATE cartable
SET RcvrRoleId = currentRole.RoleId
FROM Cartables cartable
OUTER APPLY
(
    SELECT TOP (1) userRole.RoleId
    FROM UserRoles userRole
    WHERE userRole.UserId = cartable.RcvrUserId
    ORDER BY userRole.UR_Id DESC
) currentRole
WHERE cartable.RcvrRoleId IS NULL AND cartable.IsDone = 0;

UPDATE hamesh
SET ActorFullNameSnapshot = LTRIM(RTRIM(CONCAT(ISNULL(users.FirstName, N''), N' ', ISNULL(users.LastName, N'')))),
    ActorPersonalCodeSnapshot = users.UserName,
    ActorRankTitleSnapshot = users.RankTitle,
    ActorUnitTitleSnapshot = COALESCE(NULLIF(users.UnitDutyTitle, N''), users.UnitTitle)
FROM Hameshes hamesh
INNER JOIN Users users ON users.Id = hamesh.UserId
WHERE hamesh.ActorFullNameSnapshot IS NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RoleAssignmentHistories");
            migrationBuilder.DropIndex(name: "IX_Cartables_RcvrRoleId", table: "Cartables");
            migrationBuilder.DropColumn(name: "RcvrRoleId", table: "Cartables");
            migrationBuilder.DropColumn(name: "ActorFullNameSnapshot", table: "Hameshes");
            migrationBuilder.DropColumn(name: "ActorPersonalCodeSnapshot", table: "Hameshes");
            migrationBuilder.DropColumn(name: "ActorRankTitleSnapshot", table: "Hameshes");
            migrationBuilder.DropColumn(name: "ActorUnitTitleSnapshot", table: "Hameshes");
        }
    }
}
