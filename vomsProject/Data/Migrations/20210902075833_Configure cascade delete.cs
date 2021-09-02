using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Configurecascadedelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Solutions_SolutionId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_Pages_PageId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Solutions_SolutionId",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Solutions_SolutionId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Pages_LastSavedVersionId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PublishedVersionId",
                table: "Pages");

            migrationBuilder.AddColumn<int>(
                name: "SolutionId",
                table: "Layouts",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Css",
                value: "* \r\n                        {box-sizing: border-box;} \r\n                            body {font-family: sans-serif;}");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LastSavedVersionId",
                table: "Pages",
                column: "LastSavedVersionId",
                unique: true,
                filter: "[LastSavedVersionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PublishedVersionId",
                table: "Pages",
                column: "PublishedVersionId",
                unique: true,
                filter: "[PublishedVersionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_SolutionId",
                table: "Layouts",
                column: "SolutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Solutions_SolutionId",
                table: "Images",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Layouts_Solutions_SolutionId",
                table: "Layouts",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_Pages_PageId",
                table: "PageContents",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Solutions_SolutionId",
                table: "Pages",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Solutions_SolutionId",
                table: "Permissions",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Solutions_SolutionId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_Layouts_Solutions_SolutionId",
                table: "Layouts");

            migrationBuilder.DropForeignKey(
                name: "FK_PageContents_Pages_PageId",
                table: "PageContents");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Solutions_SolutionId",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Solutions_SolutionId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Pages_LastSavedVersionId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PublishedVersionId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Layouts_SolutionId",
                table: "Layouts");

            migrationBuilder.DropColumn(
                name: "SolutionId",
                table: "Layouts");

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Css",
                value: "* {\r\n    box-sizing: border-box;\r\n}\r\n\r\nbody {\r\n    font-family: sans-serif;\r\n}\r\n");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LastSavedVersionId",
                table: "Pages",
                column: "LastSavedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PublishedVersionId",
                table: "Pages",
                column: "PublishedVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Solutions_SolutionId",
                table: "Images",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PageContents_Pages_PageId",
                table: "PageContents",
                column: "PageId",
                principalTable: "Pages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Solutions_SolutionId",
                table: "Pages",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_AspNetUsers_UserId",
                table: "Permissions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Solutions_SolutionId",
                table: "Permissions",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
