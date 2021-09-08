using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Usevomseditorsaveformat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [VOMS].[dbo].[PageContents] SET Content = '[]'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
