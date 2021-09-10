using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Addedadivconfigurationtoourseeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "StylesheetOptions",
                value: "buttonColor,The color of the button,color;headerFont,header Font,font;divDesign,This will affect all divs,div");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "StylesheetOptions",
                value: "buttonColor,The color of the button,color;headerFont,header Font,font");
        }
    }
}
