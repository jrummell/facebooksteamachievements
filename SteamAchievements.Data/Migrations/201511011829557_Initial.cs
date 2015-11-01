namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.steam_Achievement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApiName = c.String(nullable: false, maxLength: 100, unicode: false),
                        GameId = c.Int(nullable: false),
                        ImageUrl = c.String(nullable: false, maxLength: 200, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.steam_AchievementName",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AchievementId = c.Int(nullable: false),
                        Language = c.String(nullable: false, maxLength: 20, unicode: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.steam_Achievement", t => t.AchievementId)
                .Index(t => t.AchievementId);
            
            CreateTable(
                "dbo.steam_UserAchievement",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FacebookUserId = c.Long(nullable: false),
                        AchievementId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Published = c.Boolean(nullable: false),
                        Hidden = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.steam_User", t => t.FacebookUserId)
                .ForeignKey("dbo.steam_Achievement", t => t.AchievementId)
                .Index(t => t.FacebookUserId)
                .Index(t => t.AchievementId);
            
            CreateTable(
                "dbo.steam_User",
                c => new
                    {
                        FacebookUserId = c.Long(nullable: false),
                        SteamUserId = c.String(nullable: false, maxLength: 50, unicode: false),
                        AccessToken = c.String(nullable: false, maxLength: 250, unicode: false),
                        AutoUpdate = c.Boolean(nullable: false),
                        PublishDescription = c.Boolean(nullable: false),
                        Language = c.String(maxLength: 20, unicode: false),
                    })
                .PrimaryKey(t => t.FacebookUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.steam_UserAchievement", "AchievementId", "dbo.steam_Achievement");
            DropForeignKey("dbo.steam_UserAchievement", "FacebookUserId", "dbo.steam_User");
            DropForeignKey("dbo.steam_AchievementName", "AchievementId", "dbo.steam_Achievement");
            DropIndex("dbo.steam_UserAchievement", new[] { "AchievementId" });
            DropIndex("dbo.steam_UserAchievement", new[] { "FacebookUserId" });
            DropIndex("dbo.steam_AchievementName", new[] { "AchievementId" });
            DropTable("dbo.steam_User");
            DropTable("dbo.steam_UserAchievement");
            DropTable("dbo.steam_AchievementName");
            DropTable("dbo.steam_Achievement");
        }
    }
}
