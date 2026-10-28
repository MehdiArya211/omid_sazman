using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261022123000_CompleteNotificationCenter")]
    public partial class CompleteNotificationCenter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
;WITH DuplicateViews AS
(
    SELECT Id, ROW_NUMBER() OVER (PARTITION BY NotificationId, UserId ORDER BY ViewedDate, Id) AS RowNumber
    FROM NotificationUsers
)
DELETE FROM DuplicateViews WHERE RowNumber > 1;");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationUsers_NotificationId_UserId",
                table: "NotificationUsers",
                columns: new[] { "NotificationId", "UserId" },
                unique: true);

            migrationBuilder.Sql(@"
DECLARE @ParentId int;
SELECT @ParentId = PermissionId FROM Permission WHERE MenuUrl = '#notification-center';
IF @ParentId IS NULL
BEGIN
    INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES (N'اعلانات سامانه', NULL, N'/Notifications', NULL, 9050, 1, 1, 'ti-bell', '#notification-center');
    SET @ParentId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl = '/Notifications')
    INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES (N'اعلان‌های من', @ParentId, N'/Notifications', N'/Notifications', 1, 1, 1, 'ti-bell', '/Notifications');

IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl = '/Admin/Notifications')
    INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES (N'مدیریت اعلان‌ها', @ParentId, N'/Admin/Notifications', N'/Admin/Notifications', 2, 0, 1, 'ti-settings', '/Admin/Notifications');

DECLARE @UserMenuId int = (SELECT PermissionId FROM Permission WHERE MenuUrl = '/Notifications');
DECLARE @AdminMenuId int = (SELECT PermissionId FROM Permission WHERE MenuUrl = '/Admin/Notifications');

INSERT INTO RolePermission (RoleId, PermissionId)
SELECT roleItem.RoleId, permissionItem.PermissionId
FROM Roles roleItem CROSS JOIN Permission permissionItem
WHERE roleItem.IsDelete = 0
  AND permissionItem.PermissionId IN (@ParentId, @UserMenuId)
  AND NOT EXISTS (SELECT 1 FROM RolePermission existing WHERE existing.RoleId = roleItem.RoleId AND existing.PermissionId = permissionItem.PermissionId);

INSERT INTO RolePermission (RoleId, PermissionId)
SELECT roleItem.RoleId, @AdminMenuId
FROM Roles roleItem
WHERE roleItem.IsDelete = 0 AND roleItem.RoleType = 100
  AND NOT EXISTS (SELECT 1 FROM RolePermission existing WHERE existing.RoleId = roleItem.RoleId AND existing.PermissionId = @AdminMenuId);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @ParentId int = (SELECT PermissionId FROM Permission WHERE MenuUrl = '#notification-center');
DELETE FROM RolePermission WHERE PermissionId = @ParentId OR PermissionId IN (SELECT PermissionId FROM Permission WHERE ParentID = @ParentId);
DELETE FROM Permission WHERE ParentID = @ParentId;
DELETE FROM Permission WHERE PermissionId = @ParentId;");
            migrationBuilder.DropIndex(
                name: "IX_NotificationUsers_NotificationId_UserId",
                table: "NotificationUsers");
        }
    }
}
