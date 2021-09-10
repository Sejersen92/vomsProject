using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Addsolutiondefaultlayout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO  [dbo].[Layouts]
SELECT TOP (1) 'Default layout', '[]', '', '[]', '', SYSUTCDATETIME(), [UserId], [Solutions].[Id] AS [SolutionId] FROM  [VOMS].[dbo].[Solutions]
INNER JOIN
[dbo].[Permissions]
ON [SolutionId] = [dbo].[Solutions].[Id]
WHERE [PermissionLevel] = 0
");

            migrationBuilder.AddColumn<int>(
                name: "DefaultLayoutId",
                table: "Solutions",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.Sql(@"
UPDATE [dbo].[Solutions] SET [DefaultLayoutId] = [Layouts].[Id]
FROM [dbo].[Solutions]
INNER JOIN
[dbo].[Layouts]
ON [SolutionId] = [Solutions].[Id]");
            migrationBuilder.CreateIndex(
                name: "IX_Solutions_DefaultLayoutId",
                table: "Solutions",
                column: "DefaultLayoutId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Layouts_DefaultLayoutId",
                table: "Solutions",
                column: "DefaultLayoutId",
                principalTable: "Layouts",
                principalColumn: "Id");
            migrationBuilder.Sql(@"
UPDATE [dbo].[Pages] SET [LayoutId] = [Layouts].[Id]
FROM
[dbo].[Pages]
INNER JOIN
[dbo].[Solutions]
ON [SolutionId] = [Solutions].[Id]
INNER JOIN
[dbo].[Layouts]
ON [Layouts].[SolutionId] = [Solutions].[Id]");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Layouts_DefaultLayoutId",
                table: "Solutions");

            migrationBuilder.DropIndex(
                name: "IX_Solutions_DefaultLayoutId",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "DefaultLayoutId",
                table: "Solutions");
        }
    }
}
