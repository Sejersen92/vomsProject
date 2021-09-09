using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class AddedStylesheetCustomizationstoStyleAndSolutiondbos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StylesheetOptions",
                table: "Styles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerializedStylesheet",
                table: "Solutions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StylesheetCustomization",
                table: "Solutions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StylesheetOptions",
                table: "Styles");

            migrationBuilder.DropColumn(
                name: "SerializedStylesheet",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "StylesheetCustomization",
                table: "Solutions");
        }
    }
}
