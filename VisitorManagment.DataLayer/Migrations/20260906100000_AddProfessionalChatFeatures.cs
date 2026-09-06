using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using VisitorManagment.DataLayer.Context;

namespace VisitorManagment.DataLayer.Migrations
{
    [DbContext(typeof(VisitorManagmentContext))]
    [Migration("20260906100000_AddProfessionalChatFeatures")]
    public class AddProfessionalChatFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("AttachmentContentType", "ChatMessages", "nvarchar(150)", maxLength: 150, nullable: true);
            migrationBuilder.AddColumn<string>("AttachmentName", "ChatMessages", "nvarchar(260)", maxLength: 260, nullable: true);
            migrationBuilder.AddColumn<long>("AttachmentSize", "ChatMessages", "bigint", nullable: true);
            migrationBuilder.AddColumn<string>("AttachmentUrl", "ChatMessages", "nvarchar(500)", maxLength: 500, nullable: true);
            migrationBuilder.AddColumn<DateTime>("DeletedAt", "ChatMessages", "datetime2", nullable: true);
            migrationBuilder.AddColumn<DateTime>("EditedAt", "ChatMessages", "datetime2", nullable: true);
            migrationBuilder.AddColumn<bool>("IsDeleted", "ChatMessages", "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<bool>("IsDelivered", "ChatMessages", "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<DateTime>("DeliveredAt", "ChatMessages", "datetime2", nullable: true);
            migrationBuilder.AddColumn<bool>("IsRead", "ChatMessages", "bit", nullable: false, defaultValue: false);
            migrationBuilder.AddColumn<DateTime>("ReadAt", "ChatMessages", "datetime2", nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("AttachmentContentType", "ChatMessages");
            migrationBuilder.DropColumn("AttachmentName", "ChatMessages");
            migrationBuilder.DropColumn("AttachmentSize", "ChatMessages");
            migrationBuilder.DropColumn("AttachmentUrl", "ChatMessages");
            migrationBuilder.DropColumn("DeletedAt", "ChatMessages");
            migrationBuilder.DropColumn("EditedAt", "ChatMessages");
            migrationBuilder.DropColumn("IsDeleted", "ChatMessages");
            migrationBuilder.DropColumn("IsDelivered", "ChatMessages");
            migrationBuilder.DropColumn("DeliveredAt", "ChatMessages");
            migrationBuilder.DropColumn("IsRead", "ChatMessages");
            migrationBuilder.DropColumn("ReadAt", "ChatMessages");
        }
    }
}
