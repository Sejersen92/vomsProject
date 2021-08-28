using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Addversionhistorytopages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Pages");

            migrationBuilder.AddColumn<int>(
                name: "LastSavedVersionId",
                table: "Pages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublishedVersionId",
                table: "Pages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PageContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SavedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PageId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageContents_AspNetUsers_SavedById",
                        column: x => x.SavedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PageContents_Pages_PageId",
                        column: x => x.PageId,
                        principalTable: "Pages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LastSavedVersionId",
                table: "Pages",
                column: "LastSavedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PublishedVersionId",
                table: "Pages",
                column: "PublishedVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_PageId",
                table: "PageContents",
                column: "PageId");

            migrationBuilder.CreateIndex(
                name: "IX_PageContents_SavedById",
                table: "PageContents",
                column: "SavedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PageContents_LastSavedVersionId",
                table: "Pages",
                column: "LastSavedVersionId",
                principalTable: "PageContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PageContents_PublishedVersionId",
                table: "Pages",
                column: "PublishedVersionId",
                principalTable: "PageContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PageContents_LastSavedVersionId",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PageContents_PublishedVersionId",
                table: "Pages");

            migrationBuilder.DropTable(
                name: "PageContents");

            migrationBuilder.DropIndex(
                name: "IX_Pages_LastSavedVersionId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PublishedVersionId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "LastSavedVersionId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PublishedVersionId",
                table: "Pages");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Pages",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
