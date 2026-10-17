using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    /// <summary>دیتابیس‌هایی را که نسخه اشتباه ثبت حضور آنلاین روی آن‌ها اجرا شده، بدون نیاز به مداخله دستی اصلاح می‌کند.</summary>
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261005110000_CorrectInspectionParticipation")]
    public class CorrectInspectionParticipation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[InspectionAttendances]', N'U') IS NOT NULL
    DROP TABLE [InspectionAttendances];

IF OBJECT_ID(N'[InspectionMeetingPeople]', N'U') IS NULL
BEGIN
    CREATE TABLE [InspectionMeetingPeople]
    (
        [Id] int IDENTITY(1,1) NOT NULL CONSTRAINT [PK_InspectionMeetingPeople] PRIMARY KEY,
        [InspectionMeetingId] int NOT NULL,
        [InspectionPersonId] int NOT NULL,
        [RegisteredAt] datetime2 NOT NULL,
        [RegisteredByUserId] int NOT NULL,
        CONSTRAINT [FK_InspectionMeetingPeople_InspectionMeetings_InspectionMeetingId]
            FOREIGN KEY ([InspectionMeetingId]) REFERENCES [InspectionMeetings]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_InspectionMeetingPeople_InspectionPeople_InspectionPersonId]
            FOREIGN KEY ([InspectionPersonId]) REFERENCES [InspectionPeople]([Id]) ON DELETE NO ACTION
    );
    CREATE UNIQUE INDEX [IX_InspectionMeetingPeople_InspectionMeetingId_InspectionPersonId]
        ON [InspectionMeetingPeople]([InspectionMeetingId], [InspectionPersonId]);
    CREATE INDEX [IX_InspectionMeetingPeople_InspectionPersonId]
        ON [InspectionMeetingPeople]([InspectionPersonId]);
END;

DECLARE @OldPermissionId int=(SELECT TOP(1) PermissionId FROM Permission WHERE MenuUrl='/Admin/Inspection/Attendance');
IF @OldPermissionId IS NOT NULL
BEGIN
    DELETE FROM RolePermission WHERE PermissionId=@OldPermissionId;
    DELETE FROM Permission WHERE PermissionId=@OldPermissionId;
END;

DECLARE @ParentId int=(SELECT TOP(1) PermissionId FROM Permission WHERE MenuUrl='#inspection-management');
IF @ParentId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM Permission WHERE MenuUrl='/Admin/Inspection/Participation')
BEGIN
    INSERT INTO Permission (PermissionTitle,ParentID,ParentUrl,SubUrl,[Order],ShowAll,IsActive,IconName,MenuUrl)
    VALUES(N'سابقه شرکت نفرات',@ParentId,'/Inspection','/Admin/Inspection/Participation',4,0,1,'ti-time','/Admin/Inspection/Participation');
END;

DECLARE @PermissionId int=(SELECT TOP(1) PermissionId FROM Permission WHERE MenuUrl='/Admin/Inspection/Participation');
IF @PermissionId IS NOT NULL
BEGIN
    INSERT INTO RolePermission(RoleId,PermissionId)
    SELECT r.RoleId,@PermissionId FROM Roles r
    WHERE r.IsDelete=0 AND r.Title IN(N'مدیر سامانه',N'ف ق انصار نزاجا')
      AND NOT EXISTS(SELECT 1 FROM RolePermission rp WHERE rp.RoleId=r.RoleId AND rp.PermissionId=@PermissionId);
END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // بازگرداندن مدل اشتباه حضور آنلاین عمداً انجام نمی‌شود؛ این migration اصلاح داده است.
        }
    }
}
