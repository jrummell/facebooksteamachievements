using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    public class UserClaim : IdentityUserClaim<int>
    {
        public virtual steam_User User { get; set; }
    }
}