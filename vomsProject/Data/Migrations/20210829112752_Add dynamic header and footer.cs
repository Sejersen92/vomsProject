using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace vomsProject.Data.Migrations
{
    public partial class Adddynamicheaderandfooter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LayoutId",
                table: "Pages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Layouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderEditableContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterEditableContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FooterContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SaveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SavedById = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Layouts_AspNetUsers_SavedById",
                        column: x => x.SavedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LayoutId",
                table: "Pages",
                column: "LayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Layouts_SavedById",
                table: "Layouts",
                column: "SavedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Layouts_LayoutId",
                table: "Pages",
                column: "LayoutId",
                principalTable: "Layouts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Layouts_LayoutId",
                table: "Pages");

            migrationBuilder.DropTable(
                name: "Layouts");

            migrationBuilder.DropIndex(
                name: "IX_Pages_LayoutId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "LayoutId",
                table: "Pages");
        }
    }
}
