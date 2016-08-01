namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Setsteam_UserAccessTokenNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.steam_User", "AccessToken", c => c.String(maxLength: 250, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.steam_User", "AccessToken", c => c.String(nullable: false, maxLength: 250, unicode: false));
        }
    }
}
