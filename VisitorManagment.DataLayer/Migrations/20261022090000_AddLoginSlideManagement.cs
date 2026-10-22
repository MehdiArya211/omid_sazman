using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261022090000_AddLoginSlideManagement")]
    public partial class AddLoginSlideManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoginSlides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    AltText = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<System.DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<System.DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table => table.PrimaryKey("PK_LoginSlides", item => item.Id));

            migrationBuilder.CreateIndex(
                name: "IX_LoginSlides_SortOrder",
                table: "LoginSlides",
                column: "SortOrder");

            // سه تصویر فعلی صفحه ورود حفظ می‌شوند و پس از migration قابل مدیریت خواهند بود.
            migrationBuilder.Sql(@"
INSERT INTO LoginSlides (ImagePath, Title, Description, AltText, SortOrder, IsActive, CreatedAt)
VALUES
(N'/assets/media/image/photo2.jpg', N'دسترسی سریع و یکپارچه', N'ثبت و پیگیری درخواست‌ها در یک محیط سازمانی', N'محیط کاری دیجیتال', 1, 1, GETDATE()),
(N'/assets/media/image/photo3.jpg', N'پیگیری شفاف درخواست‌ها', N'مشاهده وضعیت و گردش درخواست در هر مرحله', N'کار با سامانه سازمانی', 2, 1, GETDATE()),
(N'/assets/media/image/photo1.jpg', N'مدیریت ساده‌تر', N'دسترسی به کارتابل و خدمات موردنیاز کارکنان', N'سامانه هوشمند مدیریتی', 3, 1, GETDATE());");

            migrationBuilder.Sql(@"
DECLARE @ParentId int;
SELECT @ParentId = PermissionId FROM Permission WHERE MenuUrl = '#login-page-settings';
IF @ParentId IS NULL
BEGIN
    INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES (N'تنظیمات صفحه ورود', NULL, N'/Admin/LoginSlides', NULL, 9100, 0, 1, 'ti-image', '#login-page-settings');
    SET @ParentId = SCOPE_IDENTITY();
END;

IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl = '/Admin/LoginSlides')
    INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
    VALUES (N'مدیریت تصاویر', @ParentId, N'/Admin/LoginSlides', N'/Admin/LoginSlides', 1, 0, 1, 'ti-image', '/Admin/LoginSlides');

INSERT INTO RolePermission (RoleId, PermissionId)
SELECT roleItem.RoleId, permissionItem.PermissionId
FROM Roles roleItem CROSS JOIN Permission permissionItem
WHERE roleItem.RoleType = 100 AND roleItem.IsDelete = 0
  AND (permissionItem.PermissionId = @ParentId OR permissionItem.ParentID = @ParentId)
  AND NOT EXISTS (SELECT 1 FROM RolePermission existing WHERE existing.RoleId = roleItem.RoleId AND existing.PermissionId = permissionItem.PermissionId);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DECLARE @ParentId int = (SELECT PermissionId FROM Permission WHERE MenuUrl = '#login-page-settings');
DELETE FROM RolePermission WHERE PermissionId = @ParentId OR PermissionId IN (SELECT PermissionId FROM Permission WHERE ParentID = @ParentId);
DELETE FROM Permission WHERE ParentID = @ParentId;
DELETE FROM Permission WHERE PermissionId = @ParentId;");
            migrationBuilder.DropTable(name: "LoginSlides");
        }
    }
}
