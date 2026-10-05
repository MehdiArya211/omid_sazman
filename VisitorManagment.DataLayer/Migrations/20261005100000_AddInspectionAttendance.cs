using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261005100000_AddInspectionAttendance")]
    public class AddInspectionAttendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "InspectionAttendances", columns: table => new
            {
                Id = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                InspectionMeetingId = table.Column<int>(nullable: false), PersonalCode = table.Column<string>(maxLength: 10, nullable: false),
                FullName = table.Column<string>(maxLength: 250, nullable: false), RankTitle = table.Column<string>(maxLength: 100, nullable: true),
                UnitCode = table.Column<int>(nullable: false), UnitTitle = table.Column<string>(maxLength: 200, nullable: true),
                ConnectionId = table.Column<string>(maxLength: 100, nullable: false), JoinedAt = table.Column<DateTime>(nullable: false),
                LeftAt = table.Column<DateTime>(nullable: true), DurationSeconds = table.Column<int>(nullable: false)
            }, constraints: table =>
            {
                table.PrimaryKey("PK_InspectionAttendances", x => x.Id);
                table.ForeignKey("FK_InspectionAttendances_InspectionMeetings_InspectionMeetingId", x => x.InspectionMeetingId, "InspectionMeetings", "Id", onDelete: ReferentialAction.Cascade);
            });
            migrationBuilder.CreateIndex("IX_InspectionAttendances_InspectionMeetingId_PersonalCode", "InspectionAttendances", new[] { "InspectionMeetingId", "PersonalCode" });

            migrationBuilder.Sql(@"
DECLARE @ParentId int=(SELECT PermissionId FROM Permission WHERE MenuUrl='#inspection-management');
IF @ParentId IS NOT NULL AND NOT EXISTS(SELECT 1 FROM Permission WHERE MenuUrl='/Admin/Inspection/Attendance')
 INSERT INTO Permission (PermissionTitle,ParentID,ParentUrl,SubUrl,[Order],ShowAll,IsActive,IconName,MenuUrl)
 VALUES(N'سابقه حضور نفرات',@ParentId,'/Inspection','/Admin/Inspection/Attendance',4,0,1,'ti-time','/Admin/Inspection/Attendance');
DECLARE @PermissionId int=(SELECT PermissionId FROM Permission WHERE MenuUrl='/Admin/Inspection/Attendance');
INSERT INTO RolePermission(RoleId,PermissionId)
SELECT RoleId,@PermissionId FROM Roles r WHERE r.IsDelete=0 AND r.Title IN(N'مدیر سامانه',N'ف ق انصار نزاجا')
AND NOT EXISTS(SELECT 1 FROM RolePermission rp WHERE rp.RoleId=r.RoleId AND rp.PermissionId=@PermissionId);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM RolePermission WHERE PermissionId IN(SELECT PermissionId FROM Permission WHERE MenuUrl='/Admin/Inspection/Attendance'); DELETE FROM Permission WHERE MenuUrl='/Admin/Inspection/Attendance';");
            migrationBuilder.DropTable("InspectionAttendances");
        }
    }
}
