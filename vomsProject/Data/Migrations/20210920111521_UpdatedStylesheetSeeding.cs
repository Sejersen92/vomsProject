using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class UpdatedStylesheetSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Css", "StylesheetOptions" },
                values: new object[] { "*{\r\n                        box-sizing: border-box;\r\n                        }\r\n                        body{\r\n                        }\r\n                        h1{\r\n                            color: var(--header1Color);\r\n                            font-family: var(--header1Font);\r\n                        }\r\n                        h2{\r\n                            color: var(--header2Color);\r\n                            font-family: var(--header2Font);\r\n                        }\r\n                        h3{\r\n                            color: var(--header3Color);\r\n                            font-family: var(--header3Font);\r\n                        }\r\n                        h4{\r\n                            color: var(--header4Color);\r\n                            font-family: var(--header4Font);\r\n                        }\r\n                        h5{\r\n                            color: var(--header5Color);\r\n                            font-family: var(--header5Font);\r\n                        }\r\n                        h6{\r\n                            color: var(--header6Color);\r\n                            font-family: var(--header6Font);\r\n                        }\r\n                        p{\r\n                            color: var(--paragraphColor);\r\n                            font-family: var(--paragraphFont);\r\n                        }\r\n                        b{\r\n	                        color: var(--boldColor);\r\n	                        font-family: var(--boldFont)\r\n                        }\r\n                        em{\r\n	                        color: var(--emphasisColor);\r\n	                        font-family: var(--emphasisFont)\r\n                        }", "\r\n                        header1Color,The color of the h1 element,color\r\n                        header1Font,The font of the h1 element,font\r\n                        header2Color,The color of the h2 element,color\r\n                        header2Font,The font of the h2 element,font\r\n                        header3Color,The color of the h3 element,color\r\n                        header3Font,The font of the h3 element,font\r\n                        header4Color,The color of the h4 element,color\r\n                        header4Font,The font of the h4 element,font\r\n                        header5Color,The color of the h5 element,color\r\n                        header5Font,The font of the h5 element,font\r\n                        header6Color,The color of the h6 element,color\r\n                        header6Font,The font of the h6 element,font\r\n                        paragraphColor,The color of the paragraph element,color\r\n                        paragraphFont,The font of the paragraph element,font\r\n                        boldColor,The color of the bold element,color\r\n                        boldFont,The font of the bold element,font\r\n                        emphasisColor,The color of the emphasis element,color\r\n                        emphasisFont,The font of the h6 element,font\r\n                        " });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Css", "StylesheetOptions" },
                values: new object[] { "* \r\n                        {box-sizing: border-box;} \r\n                            body {font-family: sans-serif;}", "buttonColor,The color of the button,color;headerFont,header Font,font;divDesign,This will affect all divs,font" });
        }
    }
}
