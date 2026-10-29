using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261029090000_AllowSharedInspectionRoleHistory")]
    public class AllowSharedInspectionRoleHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_RoleAssignmentHistories_RoleId", table: "RoleAssignmentHistories");
            migrationBuilder.DropIndex(name: "IX_RoleAssignmentHistories_UserId", table: "RoleAssignmentHistories");
            migrationBuilder.CreateIndex(name: "IX_RoleAssignmentHistories_RoleId", table: "RoleAssignmentHistories", column: "RoleId");
            migrationBuilder.CreateIndex(name: "IX_RoleAssignmentHistories_UserId", table: "RoleAssignmentHistories", column: "UserId");
            // یک نفر نباید برای همان نقش دو سابقه فعال داشته باشد؛ چند نفر برای یک نقش مجازند.
            migrationBuilder.CreateIndex(name: "IX_RoleAssignmentHistories_RoleId_UserId",
                table: "RoleAssignmentHistories", columns: new[] { "RoleId", "UserId" },
                unique: true, filter: "[EndDate] IS NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // بازگشت در صورت وجود چند متصدی متوقف می‌شود؛ هیچ سابقه‌ای حذف نمی‌شود.
            migrationBuilder.Sql(@"IF EXISTS (SELECT RoleId FROM RoleAssignmentHistories WHERE EndDate IS NULL GROUP BY RoleId HAVING COUNT(*) > 1)
OR EXISTS (SELECT UserId FROM RoleAssignmentHistories WHERE EndDate IS NULL GROUP BY UserId HAVING COUNT(*) > 1)
THROW 51000, N'بازگشت ممکن نیست؛ ابتدا سوابق فعال چندگانه را بررسی کنید.', 1;");
            migrationBuilder.DropIndex(name: "IX_RoleAssignmentHistories_RoleId_UserId", table: "RoleAssignmentHistories");
            migrationBuilder.DropIndex(name: "IX_RoleAssignmentHistories_RoleId", table: "RoleAssignmentHistories");
            migrationBuilder.DropIndex(name: "IX_RoleAssignmentHistories_UserId", table: "RoleAssignmentHistories");
            migrationBuilder.CreateIndex(name: "IX_RoleAssignmentHistories_RoleId", table: "RoleAssignmentHistories", column: "RoleId", unique: true, filter: "[EndDate] IS NULL");
            migrationBuilder.CreateIndex(name: "IX_RoleAssignmentHistories_UserId", table: "RoleAssignmentHistories", column: "UserId", unique: true, filter: "[EndDate] IS NULL");
        }
    }
}