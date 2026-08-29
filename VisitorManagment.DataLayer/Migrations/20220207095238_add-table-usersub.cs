using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class addtableusersub : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserSubId",
                table: "UserRoles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSubId",
                table: "Hameshes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserSub",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FPrsnNo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JobDes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitDutyCode = table.Column<int>(type: "int", nullable: true),
                    UnitCode = table.Column<int>(type: "int", nullable: true),
                    UnitDutyTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodGha = table.Column<int>(type: "int", nullable: true),
                    CodGhaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSub", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserSubId",
                table: "UserRoles",
                column: "UserSubId");

            migrationBuilder.CreateIndex(
                name: "IX_Hameshes_UserSubId",
                table: "Hameshes",
                column: "UserSubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hameshes_UserSub_UserSubId",
                table: "Hameshes",
                column: "UserSubId",
                principalTable: "UserSub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_UserSub_UserSubId",
                table: "UserRoles",
                column: "UserSubId",
                principalTable: "UserSub",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hameshes_UserSub_UserSubId",
                table: "Hameshes");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_UserSub_UserSubId",
                table: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSub");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UserSubId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_Hameshes_UserSubId",
                table: "Hameshes");

            migrationBuilder.DropColumn(
                name: "UserSubId",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UserSubId",
                table: "Hameshes");
        }
    }
}
