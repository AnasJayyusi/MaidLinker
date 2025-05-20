using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class EditTaleLangName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LangaugeMaid");

            migrationBuilder.CreateTable(
                name: "LanguagesMaid",
                columns: table => new
                {
                    LangaugesId = table.Column<int>(type: "int", nullable: false),
                    MaidsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguagesMaid", x => new { x.LangaugesId, x.MaidsId });
                    table.ForeignKey(
                        name: "FK_LanguagesMaid_Langauges_LangaugesId",
                        column: x => x.LangaugesId,
                        principalTable: "Langauges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguagesMaid_Maids_MaidsId",
                        column: x => x.MaidsId,
                        principalTable: "Maids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguagesMaid_MaidsId",
                table: "LanguagesMaid",
                column: "MaidsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguagesMaid");

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

            migrationBuilder.CreateIndex(
                name: "IX_LangaugeMaid_MaidsId",
                table: "LangaugeMaid",
                column: "MaidsId");
        }
    }
}
