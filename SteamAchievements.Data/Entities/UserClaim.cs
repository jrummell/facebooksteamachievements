using Microsoft.AspNetCore.Identity;

namespace SteamAchievements.Data
{
    public class UserClaim : IdentityUserClaim<int>
    {
        public virtual User User { get; set; }
    }
}