using Microsoft.EntityFrameworkCore.Migrations;

namespace VisitorManagment.DataLayer.Migrations
{
    public partial class mig_Edit_Table_Personal2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotAml2",
                table: "Personals",
                newName: "TOT_AML2");

            migrationBuilder.RenameColumn(
                name: "TotAml",
                table: "Personals",
                newName: "TOT_AML");

            migrationBuilder.RenameColumn(
                name: "Drsa_Jb",
                table: "Personals",
                newName: "DRSAD_JB");

            migrationBuilder.RenameColumn(
                name: "Drsa_Ja",
                table: "Personals",
                newName: "DRSAD_JA");

            migrationBuilder.RenameColumn(
                name: "TotAml2",
                table: "Files",
                newName: "TOT_AML2");

            migrationBuilder.RenameColumn(
                name: "TotAml",
                table: "Files",
                newName: "TOT_AML");

            migrationBuilder.RenameColumn(
                name: "Drsa_Jb",
                table: "Files",
                newName: "DRSAD_JB");

            migrationBuilder.RenameColumn(
                name: "Drsa_Ja",
                table: "Files",
                newName: "DRSAD_JA");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TOT_AML2",
                table: "Personals",
                newName: "TotAml2");

            migrationBuilder.RenameColumn(
                name: "TOT_AML",
                table: "Personals",
                newName: "TotAml");

            migrationBuilder.RenameColumn(
                name: "DRSAD_JB",
                table: "Personals",
                newName: "Drsa_Jb");

            migrationBuilder.RenameColumn(
                name: "DRSAD_JA",
                table: "Personals",
                newName: "Drsa_Ja");

            migrationBuilder.RenameColumn(
                name: "TOT_AML2",
                table: "Files",
                newName: "TotAml2");

            migrationBuilder.RenameColumn(
                name: "TOT_AML",
                table: "Files",
                newName: "TotAml");

            migrationBuilder.RenameColumn(
                name: "DRSAD_JB",
                table: "Files",
                newName: "Drsa_Jb");

            migrationBuilder.RenameColumn(
                name: "DRSAD_JA",
                table: "Files",
                newName: "Drsa_Ja");
        }
    }
}
