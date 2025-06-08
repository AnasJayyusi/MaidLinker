using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class AddValueToGeneralSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SettingValue",
                table: "GeneralSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SettingValue",
                table: "GeneralSettings");
        }
    }
}
