using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20260907090000_AddDashboardAccess")]
    public partial class AddDashboardAccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasDashboardAccess",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl = '#management-dashboard')
BEGIN
    INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES (N'داشبورد مدیریتی', NULL, NULL, NULL, 9999, 0, 0, 'ti-bar-chart', '#management-dashboard')
END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM RolePermission WHERE PermissionId IN (SELECT PermissionId FROM Permission WHERE MenuUrl = '#management-dashboard')");
            migrationBuilder.Sql("DELETE FROM Permission WHERE MenuUrl = '#management-dashboard'");
            migrationBuilder.DropColumn(name: "HasDashboardAccess", table: "Users");
        }
    }
}
