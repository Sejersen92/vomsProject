using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Removenotnullconstraintfromdefaultlayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Solutions_DefaultLayoutId",
                table: "Solutions");

            migrationBuilder.AlterColumn<int>(
                name: "DefaultLayoutId",
                table: "Solutions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_DefaultLayoutId",
                table: "Solutions",
                column: "DefaultLayoutId",
                unique: true,
                filter: "[DefaultLayoutId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Solutions_DefaultLayoutId",
                table: "Solutions");

            migrationBuilder.AlterColumn<int>(
                name: "DefaultLayoutId",
                table: "Solutions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_DefaultLayoutId",
                table: "Solutions",
                column: "DefaultLayoutId",
                unique: true);
        }
    }
}
