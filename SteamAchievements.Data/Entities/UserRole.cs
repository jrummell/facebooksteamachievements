using Microsoft.AspNet.Identity.EntityFramework;

namespace SteamAchievements.Data
{
    public class UserRole : IdentityUserRole<int>
    {
        public int Id { get; set; }

        public virtual steam_User User { get; set; }
        public virtual Role Role { get; set; }
    }
}