using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Addmimetypetoimages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Images",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Images");
        }
    }
}
