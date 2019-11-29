using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SteamAchievements.Data
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> Users { get; set; }
    }
}