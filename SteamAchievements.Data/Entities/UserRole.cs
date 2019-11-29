using Microsoft.AspNetCore.Identity;

namespace SteamAchievements.Data
{
    public class UserRole : IdentityUserRole<int>
    {
        public int Id { get; set; }

        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}