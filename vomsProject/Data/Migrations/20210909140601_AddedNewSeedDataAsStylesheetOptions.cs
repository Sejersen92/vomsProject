using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class AddedNewSeedDataAsStylesheetOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "StylesheetOptions",
                value: "buttoncolor,#FFFFFF,knappens farve,color;viktorsfisse,#ba2222,viktors numse,color;headerFont,calibri,header Font,font");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "StylesheetOptions",
                value: "");
        }
    }
}
