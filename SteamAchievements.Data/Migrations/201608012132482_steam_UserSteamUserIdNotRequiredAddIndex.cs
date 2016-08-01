namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class steam_UserSteamUserIdNotRequiredAddIndex : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.steam_User", "SteamUserId", c => c.String(maxLength: 50, unicode: false));
            CreateIndex("dbo.steam_User", "SteamUserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.steam_User", new[] { "SteamUserId" });
            AlterColumn("dbo.steam_User", "SteamUserId", c => c.String(nullable: false, maxLength: 50, unicode: false));
        }
    }
}
