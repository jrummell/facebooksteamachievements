namespace SteamAchievements.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFacebookLogins : DbMigration
    {
        public override void Up()
        {
            Sql(@"delete from [UserLogin]

insert into [UserLogin] ([LoginProvider], [ProviderKey], [UserId])
select 'Facebook', cast(FacebookUserId as varchar), Id
from steam_User");
        }
        
        public override void Down()
        {
        }
    }
}
