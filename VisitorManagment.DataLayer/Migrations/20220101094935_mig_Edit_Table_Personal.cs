using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_Edit_Table_Personal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmaliatiKhedmate",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "GhableGhatnameAmaliatiKhedmate",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "AmaliatiKhedmate",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "GhableGhatnameAmaliatiKhedmate",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "JanbaziBonyad",
                table: "Personals",
                newName: "TotAml2");

            migrationBuilder.RenameColumn(
                name: "JanbaziArtesh",
                table: "Personals",
                newName: "TotAml");

            migrationBuilder.RenameColumn(
                name: "JanbaziBonyad",
                table: "Files",
                newName: "TotAml2");

            migrationBuilder.RenameColumn(
                name: "JanbaziArtesh",
                table: "Files",
                newName: "TotAml");

            migrationBuilder.AddColumn<decimal>(
                name: "Drsa_Ja",
                table: "Personals",
                type: "decimal(18,2)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Drsa_Jb",
                table: "Personals",
                type: "decimal(18,2)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Drsa_Ja",
                table: "Files",
                type: "decimal(18,2)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Drsa_Jb",
                table: "Files",
                type: "decimal(18,2)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Drsa_Ja",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "Drsa_Jb",
                table: "Personals");

            migrationBuilder.DropColumn(
                name: "Drsa_Ja",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Drsa_Jb",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "TotAml2",
                table: "Personals",
                newName: "JanbaziBonyad");

            migrationBuilder.RenameColumn(
                name: "TotAml",
                table: "Personals",
                newName: "JanbaziArtesh");

            migrationBuilder.RenameColumn(
                name: "TotAml2",
                table: "Files",
                newName: "JanbaziBonyad");

            migrationBuilder.RenameColumn(
                name: "TotAml",
                table: "Files",
                newName: "JanbaziArtesh");

            migrationBuilder.AddColumn<string>(
                name: "AmaliatiKhedmate",
                table: "Personals",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhableGhatnameAmaliatiKhedmate",
                table: "Personals",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmaliatiKhedmate",
                table: "Files",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhableGhatnameAmaliatiKhedmate",
                table: "Files",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
