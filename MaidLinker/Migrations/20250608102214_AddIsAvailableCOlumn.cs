using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class AddIsAvailableCOlumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Maids",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Maids");
        }
    }
}
