using Microsoft.AspNetCore.Identity;

namespace SteamAchievements.Data
{
    public class UserLogin : IdentityUserLogin<int>
    {
        public int Id { get; set; }

        public virtual User User { get; set; }
    }
}