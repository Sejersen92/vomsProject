using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class AddedProductVersionEnumtoUsertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions");

            migrationBuilder.AlterColumn<int>(
                name: "StyleId",
                table: "Solutions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ProductVersion",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions",
                column: "StyleId",
                principalTable: "Styles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "ProductVersion",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "StyleId",
                table: "Solutions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Styles_StyleId",
                table: "Solutions",
                column: "StyleId",
                principalTable: "Styles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
