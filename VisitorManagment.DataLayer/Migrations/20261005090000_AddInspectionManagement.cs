using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261005090000_AddInspectionManagement")]
    public class AddInspectionManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "InspectionMeetings", columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(maxLength: 200, nullable: false), Description = table.Column<string>(maxLength: 1000, nullable: true),
                MeetingDate = table.Column<string>(maxLength: 10, nullable: false), StartTime = table.Column<string>(maxLength: 5, nullable: false),
                Status = table.Column<int>(nullable: false), IsActive = table.Column<bool>(nullable: false), IsDeleted = table.Column<bool>(nullable: false),
                RegisteredAt = table.Column<DateTime>(nullable: false), RegisteredByUserId = table.Column<int>(nullable: false)
            }, constraints: table => table.PrimaryKey("PK_InspectionMeetings", x => x.Id));

            migrationBuilder.CreateTable(name: "InspectionPeople", columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"), PersonalCode = table.Column<string>(maxLength: 10, nullable: false),
                FirstName = table.Column<string>(maxLength: 100, nullable: false), LastName = table.Column<string>(maxLength: 150, nullable: false),
                RankCode = table.Column<int>(nullable: true), RankTitle = table.Column<string>(maxLength: 100, nullable: true), BranchCode = table.Column<int>(nullable: true), BranchTitle = table.Column<string>(maxLength: 150, nullable: true),
                UnitCode = table.Column<int>(nullable: true), UnitTitle = table.Column<string>(maxLength: 200, nullable: true), UnitDutyCode = table.Column<int>(nullable: true), UnitDutyTitle = table.Column<string>(maxLength: 200, nullable: true),
                JobDescription = table.Column<string>(maxLength: 250, nullable: true), RegionalCommandCode = table.Column<int>(nullable: true), RegionalCommandTitle = table.Column<string>(maxLength: 200, nullable: true),
                IsActive = table.Column<bool>(nullable: false), IsDeleted = table.Column<bool>(nullable: false), RegisteredAt = table.Column<DateTime>(nullable: false), RegisteredByUserId = table.Column<int>(nullable: false)
            }, constraints: table => table.PrimaryKey("PK_InspectionPeople", x => x.Id));

            migrationBuilder.CreateTable(name: "InspectionMeetingUnits", columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"), InspectionMeetingId = table.Column<int>(nullable: false),
                UnitCode = table.Column<int>(nullable: false), UnitTitle = table.Column<string>(maxLength: 200, nullable: false), RegisteredAt = table.Column<DateTime>(nullable: false), RegisteredByUserId = table.Column<int>(nullable: false)
            }, constraints: table => { table.PrimaryKey("PK_InspectionMeetingUnits", x => x.Id); table.ForeignKey("FK_InspectionMeetingUnits_InspectionMeetings_InspectionMeetingId", x => x.InspectionMeetingId, "InspectionMeetings", "Id", onDelete: ReferentialAction.Cascade); });

            migrationBuilder.CreateIndex("IX_InspectionPeople_PersonalCode", "InspectionPeople", "PersonalCode", unique: true);
            migrationBuilder.CreateIndex("IX_InspectionMeetingUnits_InspectionMeetingId_UnitCode", "InspectionMeetingUnits", new[] { "InspectionMeetingId", "UnitCode" }, unique: true);

            migrationBuilder.Sql(@"
DECLARE @ParentId int;
SELECT @ParentId = PermissionId FROM Permission WHERE MenuUrl = '#inspection-management';
IF @ParentId IS NULL BEGIN
 INSERT INTO Permission (PermissionTitle, ParentID, ParentUrl, SubUrl, [Order], ShowAll, IsActive, IconName, MenuUrl)
 VALUES (N'مدیریت بازرسی', NULL, '/Inspection', NULL, 850, 0, 1, 'ti-clipboard', '#inspection-management');
 SET @ParentId = SCOPE_IDENTITY();
END;
IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl='/Admin/Inspection/People') INSERT INTO Permission (PermissionTitle,ParentID,ParentUrl,SubUrl,[Order],ShowAll,IsActive,IconName,MenuUrl) VALUES (N'افزودن نفرات', @ParentId, '/Inspection', '/Admin/Inspection/People', 1, 0, 1, 'ti-user', '/Admin/Inspection/People');
IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl='/Admin/Inspection/Meetings') INSERT INTO Permission (PermissionTitle,ParentID,ParentUrl,SubUrl,[Order],ShowAll,IsActive,IconName,MenuUrl) VALUES (N'ثبت جلسه', @ParentId, '/Inspection', '/Admin/Inspection/Meetings', 2, 0, 1, 'ti-calendar', '/Admin/Inspection/Meetings');
IF NOT EXISTS (SELECT 1 FROM Permission WHERE MenuUrl='/OnlineConversation/InspectionUserMeet') INSERT INTO Permission (PermissionTitle,ParentID,ParentUrl,SubUrl,[Order],ShowAll,IsActive,IconName,MenuUrl) VALUES (N'برگزاری جلسه', @ParentId, '/Inspection', '/OnlineConversation/InspectionUserMeet', 3, 0, 1, 'ti-video-camera', '/OnlineConversation/InspectionUserMeet');

INSERT INTO RolePermission (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId FROM Roles r CROSS JOIN Permission p
WHERE r.IsDelete=0 AND r.Title IN (N'مدیر سامانه', N'ف ق انصار نزاجا')
AND (p.PermissionId=@ParentId OR p.ParentID=@ParentId)
AND NOT EXISTS (SELECT 1 FROM RolePermission rp WHERE rp.RoleId=r.RoleId AND rp.PermissionId=p.PermissionId);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DECLARE @ParentId int= (SELECT PermissionId FROM Permission WHERE MenuUrl='#inspection-management'); DELETE FROM RolePermission WHERE PermissionId=@ParentId OR PermissionId IN (SELECT PermissionId FROM Permission WHERE ParentID=@ParentId); DELETE FROM Permission WHERE ParentID=@ParentId; DELETE FROM Permission WHERE PermissionId=@ParentId;");
            migrationBuilder.DropTable("InspectionMeetingUnits");
            migrationBuilder.DropTable("InspectionPeople");
            migrationBuilder.DropTable("InspectionMeetings");
        }
    }
}
