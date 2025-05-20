using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class EditTaleLangNameAgain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguagesMaid");

            migrationBuilder.DropTable(
                name: "Langauges");

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LanguageMaid",
                columns: table => new
                {
                    LangaugesId = table.Column<int>(type: "int", nullable: false),
                    MaidsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageMaid", x => new { x.LangaugesId, x.MaidsId });
                    table.ForeignKey(
                        name: "FK_LanguageMaid_Languages_LangaugesId",
                        column: x => x.LangaugesId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LanguageMaid_Maids_MaidsId",
                        column: x => x.MaidsId,
                        principalTable: "Maids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LanguageMaid_MaidsId",
                table: "LanguageMaid",
                column: "MaidsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageMaid");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.CreateTable(
                name: "Langauges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Langauges", x => x.Id);
                });

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
    }
}
