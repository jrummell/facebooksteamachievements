namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetUserName : DbMigration
    {
        public override void Up()
        {
            Sql(@"update [steam_User] set UserName = cast(FacebookUserId as varchar)");
        }
        
        public override void Down()
        {
        }
    }
}
