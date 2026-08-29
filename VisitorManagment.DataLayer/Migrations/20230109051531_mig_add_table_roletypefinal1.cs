using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_add_table_roletypefinal1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleTypeFinalId",
                table: "Roles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoleTypeFinals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    SortNum = table.Column<int>(type: "int", nullable: true),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleTypeFinals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleTypeFinalId",
                table: "Roles",
                column: "RoleTypeFinalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_RoleTypeFinals_RoleTypeFinalId",
                table: "Roles",
                column: "RoleTypeFinalId",
                principalTable: "RoleTypeFinals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_RoleTypeFinals_RoleTypeFinalId",
                table: "Roles");

            migrationBuilder.DropTable(
                name: "RoleTypeFinals");

            migrationBuilder.DropIndex(
                name: "IX_Roles_RoleTypeFinalId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleTypeFinalId",
                table: "Roles");
        }
    }
}
