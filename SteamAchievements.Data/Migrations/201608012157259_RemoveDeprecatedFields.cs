namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDeprecatedFields : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.steam_User", new[] { "FacebookUserId" });
            DropIndex("dbo.steam_UserAchievement", new[] { "FacebookUserId" });

            Sql("DROP INDEX [IX_steam_UserAchievement_FacebookUserId] ON [dbo].[steam_UserAchievement]");

            DropColumn("dbo.steam_UserAchievement", "FacebookUserId");
            DropColumn("dbo.steam_User", "FacebookUserId");
            DropColumn("dbo.steam_User", "AccessToken");
            DropColumn("dbo.steam_User", "AutoUpdate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.steam_User", "AutoUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.steam_User", "AccessToken", c => c.String(maxLength: 250, unicode: false));
            AddColumn("dbo.steam_User", "FacebookUserId", c => c.Long(nullable: false));
            AddColumn("dbo.steam_UserAchievement", "FacebookUserId", c => c.Long(nullable: false));
            CreateIndex("dbo.steam_User", "FacebookUserId", unique: true);
        }
    }
}
