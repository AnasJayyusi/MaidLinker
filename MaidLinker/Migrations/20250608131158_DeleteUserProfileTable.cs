using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaidLinker.Migrations
{
    public partial class DeleteUserProfileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserProfiles_AssignedToUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_UserProfiles_CreatedByUserId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_AssignedToUserId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CreatedByUserId",
                table: "Notifications");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToUserId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CreatedByUserId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AssignedToUserId",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    PractitionerTypeId = table.Column<int>(type: "int", nullable: true),
                    AccountTypeId = table.Column<int>(type: "int", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", maxLength: 4096, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfilePicturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfileStatus = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfiles_PractitionerTypes_PractitionerTypeId",
                        column: x => x.PractitionerTypeId,
                        principalTable: "PractitionerTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AssignedToUserId",
                table: "Notifications",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedByUserId",
                table: "Notifications",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CountryId",
                table: "UserProfiles",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_PractitionerTypeId",
                table: "UserProfiles",
                column: "PractitionerTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserProfiles_AssignedToUserId",
                table: "Notifications",
                column: "AssignedToUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_UserProfiles_CreatedByUserId",
                table: "Notifications",
                column: "CreatedByUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
