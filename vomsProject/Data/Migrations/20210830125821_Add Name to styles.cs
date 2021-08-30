using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class AddNametostyles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Styles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StyleId",
                table: "Solutions",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "Css", "Name" },
                values: new object[] { 1, "* {\r\n    box-sizing: border-box;\r\n}\r\n\r\nbody {\r\n    font-family: sans-serif;\r\n}\r\n", "Style 1" });

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions",
                column: "StyleId",
                principalTable: "Styles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions");

            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Styles");

            migrationBuilder.AlterColumn<int>(
                name: "StyleId",
                table: "Solutions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions",
                column: "StyleId",
                principalTable: "Styles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
