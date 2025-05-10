using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class AddNationaltiesTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maids_Langauges_LangaugesId",
                table: "Maids");

            migrationBuilder.RenameColumn(
                name: "LangaugesId",
                table: "Maids",
                newName: "NationalityId");

            migrationBuilder.RenameIndex(
                name: "IX_Maids_LangaugesId",
                table: "Maids",
                newName: "IX_Maids_NationalityId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Maids",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "LangaugeMaid",
                columns: table => new
                {
                    LangaugesId = table.Column<int>(type: "int", nullable: false),
                    MaidsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LangaugeMaid", x => new { x.LangaugesId, x.MaidsId });
                    table.ForeignKey(
                        name: "FK_LangaugeMaid_Langauges_LangaugesId",
                        column: x => x.LangaugesId,
                        principalTable: "Langauges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LangaugeMaid_Maids_MaidsId",
                        column: x => x.MaidsId,
                        principalTable: "Maids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Nationalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TitleEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nationalities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LangaugeMaid_MaidsId",
                table: "LangaugeMaid",
                column: "MaidsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maids_Nationalities_NationalityId",
                table: "Maids",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maids_Nationalities_NationalityId",
                table: "Maids");

            migrationBuilder.DropTable(
                name: "LangaugeMaid");

            migrationBuilder.DropTable(
                name: "Nationalities");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Maids");

            migrationBuilder.RenameColumn(
                name: "NationalityId",
                table: "Maids",
                newName: "LangaugesId");

            migrationBuilder.RenameIndex(
                name: "IX_Maids_NationalityId",
                table: "Maids",
                newName: "IX_Maids_LangaugesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maids_Langauges_LangaugesId",
                table: "Maids",
                column: "LangaugesId",
                principalTable: "Langauges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
