using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamAchievements.Data.Migrations
{
    public partial class SteamUserIdNotUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SteamUserId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SteamUserId",
                table: "AspNetUsers",
                column: "SteamUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_SteamUserId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_SteamUserId",
                table: "AspNetUsers",
                column: "SteamUserId",
                unique: true,
                filter: "[SteamUserId] IS NOT NULL");
        }
    }
}
