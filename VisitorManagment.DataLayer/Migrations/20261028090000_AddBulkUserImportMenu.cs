using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    /// <summary>
    /// زیرمنوی ورود گروهی کاربران را کنار منوی فعلی مدیریت کاربران ایجاد می‌کند
    /// و دسترسی نقش‌ها را از همان منوی مرجع کپی می‌کند.
    /// </summary>
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261028090000_AddBulkUserImportMenu")]
    public partial class AddBulkUserImportMenu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DECLARE @SourcePermissionId INT;
DECLARE @ParentPermissionId INT;
DECLARE @NewPermissionId INT;

SELECT TOP (1)
    @SourcePermissionId = PermissionId,
    @ParentPermissionId = ParentID
FROM Permission
WHERE SubUrl IN ('/Admin/Users', '/Admin/Users/Index')
   OR MenuUrl IN ('/Admin/Users', '/Admin/Users/Index')
ORDER BY CASE WHEN SubUrl = '/Admin/Users' THEN 0 ELSE 1 END, PermissionId;

SELECT @NewPermissionId = PermissionId
FROM Permission
WHERE MenuUrl = '/Admin/Users/BulkImport'
   OR SubUrl = '/Admin/Users/BulkImport';

IF @NewPermissionId IS NULL
BEGIN
    INSERT INTO Permission
        (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES
        (N'ورود گروهی کاربران', @ParentPermissionId, NULL, '/Admin/Users/BulkImport',
         COALESCE((SELECT [Order] + 1 FROM Permission WHERE PermissionId = @SourcePermissionId), 9998),
         0, 1, 'ti-upload', '/Admin/Users/BulkImport');

    SET @NewPermissionId = SCOPE_IDENTITY();
END;

-- هر نقشی که منوی فعلی کاربران را دارد، ورود گروهی را نیز مشاهده می‌کند.
IF @SourcePermissionId IS NOT NULL
BEGIN
    INSERT INTO RolePermission (RoleId, PermissionId)
    SELECT source.RoleId, @NewPermissionId
    FROM RolePermission source
    WHERE source.PermissionId = @SourcePermissionId
      AND NOT EXISTS
      (
          SELECT 1 FROM RolePermission currentAccess
          WHERE currentAccess.RoleId = source.RoleId
            AND currentAccess.PermissionId = @NewPermissionId
      );
END;

-- در بانک‌هایی که منوی مرجع قدیمی پیدا نشود، دسترسی مدیر سامانه حفظ می‌شود.
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT role.RoleId, @NewPermissionId
FROM Roles role
WHERE role.RoleType = 100
  AND role.IsDelete = 0
  AND NOT EXISTS
  (
      SELECT 1 FROM RolePermission currentAccess
      WHERE currentAccess.RoleId = role.RoleId
        AND currentAccess.PermissionId = @NewPermissionId
  );
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM RolePermission
WHERE PermissionId IN
(
    SELECT PermissionId FROM Permission
    WHERE MenuUrl = '/Admin/Users/BulkImport'
       OR SubUrl = '/Admin/Users/BulkImport'
);

DELETE FROM Permission
WHERE MenuUrl = '/Admin/Users/BulkImport'
   OR SubUrl = '/Admin/Users/BulkImport';
");
        }
    }
}
