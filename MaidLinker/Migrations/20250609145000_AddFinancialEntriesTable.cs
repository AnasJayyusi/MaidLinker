using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class AddFinancialEntriesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PractitionerTypes_PractitionerTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "PractitionerTypes");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PractitionerTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PractitionerTypeId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "FinancialEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialEntries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialEntries");

            migrationBuilder.AddColumn<int>(
                name: "PractitionerTypeId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PractitionerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TitleEn = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PractitionerTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PractitionerTypeId",
                table: "AspNetUsers",
                column: "PractitionerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PractitionerTypes_PractitionerTypeId",
                table: "AspNetUsers",
                column: "PractitionerTypeId",
                principalTable: "PractitionerTypes",
                principalColumn: "Id");
        }
    }
}
