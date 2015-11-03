using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    public class UserLogin : IdentityUserLogin<int>
    {
        public int Id { get; set; }

        public virtual steam_User User { get; set; }
    }
}