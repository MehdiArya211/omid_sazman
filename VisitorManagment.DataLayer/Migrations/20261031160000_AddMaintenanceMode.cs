using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20261031160000_AddMaintenanceMode")]
    public class AddMaintenanceMode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenanceSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Revision = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceSettings", item => item.Id);
                    table.CheckConstraint("CK_MaintenanceSettings_SingleRow", "[Id] = 1");
                });
            migrationBuilder.InsertData(table: "MaintenanceSettings",
                columns: new[] { "Id", "IsEnabled", "Message", "Revision" },
                columnTypes: new[] { "int", "bit", "nvarchar(1000)", "int" },
                values: new object[] { 1, false, "سامانه موقتاً در حال به‌روزرسانی است. لطفاً کمی بعد دوباره مراجعه کنید.", 0 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MaintenanceSettings");
        }
    }
}
