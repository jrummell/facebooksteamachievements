using System;

namespace SteamAchievements.Services
{
    public class Achievement
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Uri ImageUrl { get; set; }

        public bool Closed { get; set; }

        public Game Game { get; set; }
    }
}
