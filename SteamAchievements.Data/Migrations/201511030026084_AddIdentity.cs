namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdentity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.steam_UserAchievement", "FacebookUserId", "dbo.steam_User");
            DropIndex("dbo.steam_UserAchievement", new[] { "FacebookUserId" });
            DropPrimaryKey("dbo.steam_User");
            CreateTable(
                "dbo.UserClaim",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.steam_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserLogin",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginProvider = c.String(),
                        ProviderKey = c.String(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.steam_User", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Role", t => t.RoleId)
                .ForeignKey("dbo.steam_User", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.steam_UserAchievement", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.steam_User", "Id", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.steam_User", "Email", c => c.String());
            AddColumn("dbo.steam_User", "EmailConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.steam_User", "PasswordHash", c => c.String());
            AddColumn("dbo.steam_User", "SecurityStamp", c => c.String());
            AddColumn("dbo.steam_User", "PhoneNumber", c => c.String());
            AddColumn("dbo.steam_User", "PhoneNumberConfirmed", c => c.Boolean(nullable: false));
            AddColumn("dbo.steam_User", "TwoFactorEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.steam_User", "LockoutEndDateUtc", c => c.DateTime());
            AddColumn("dbo.steam_User", "LockoutEnabled", c => c.Boolean(nullable: false));
            AddColumn("dbo.steam_User", "AccessFailedCount", c => c.Int(nullable: false));
            AddColumn("dbo.steam_User", "UserName", c => c.String());
            AddPrimaryKey("dbo.steam_User", "Id");
            CreateIndex("dbo.steam_UserAchievement", "UserId");
            CreateIndex("dbo.steam_User", "FacebookUserId", unique: true);
            AddForeignKey("dbo.steam_UserAchievement", "UserId", "dbo.steam_User", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.steam_UserAchievement", "UserId", "dbo.steam_User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.steam_User");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.steam_User");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.steam_User");
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.steam_User", new[] { "FacebookUserId" });
            DropIndex("dbo.steam_UserAchievement", new[] { "UserId" });
            DropPrimaryKey("dbo.steam_User");
            DropColumn("dbo.steam_User", "UserName");
            DropColumn("dbo.steam_User", "AccessFailedCount");
            DropColumn("dbo.steam_User", "LockoutEnabled");
            DropColumn("dbo.steam_User", "LockoutEndDateUtc");
            DropColumn("dbo.steam_User", "TwoFactorEnabled");
            DropColumn("dbo.steam_User", "PhoneNumberConfirmed");
            DropColumn("dbo.steam_User", "PhoneNumber");
            DropColumn("dbo.steam_User", "SecurityStamp");
            DropColumn("dbo.steam_User", "PasswordHash");
            DropColumn("dbo.steam_User", "EmailConfirmed");
            DropColumn("dbo.steam_User", "Email");
            DropColumn("dbo.steam_User", "Id");
            DropColumn("dbo.steam_UserAchievement", "UserId");
            DropTable("dbo.Role");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserClaim");
            AddPrimaryKey("dbo.steam_User", "FacebookUserId");
            CreateIndex("dbo.steam_UserAchievement", "FacebookUserId");
            AddForeignKey("dbo.steam_UserAchievement", "FacebookUserId", "dbo.steam_User", "FacebookUserId");
        }
    }
}
