using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Storemimetypeofthefavicon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FaviconMimeType",
                table: "Solutions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaviconMimeType",
                table: "Solutions");
        }
    }
}
