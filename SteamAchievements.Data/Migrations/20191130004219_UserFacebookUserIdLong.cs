using Microsoft.EntityFrameworkCore.Migrations;

namespace SteamAchievements.Data.Migrations
{
    public partial class UserFacebookUserIdLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "FacebookUserId",
                table: "User",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FacebookUserId",
                table: "User",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
