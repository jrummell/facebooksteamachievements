using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    public class UserClaim : IdentityUserClaim<int>
    {
        public virtual User User { get; set; }
    }
}