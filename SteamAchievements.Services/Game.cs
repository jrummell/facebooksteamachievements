using System;

namespace SteamAchievements.Services
{
    public class Game
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public Uri ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the stats URL.
        /// </summary>
        /// <value>The stats URL.</value>
        public Uri StatsUrl { get; set; }

        /// <summary>
        /// Gets or sets the store URL.
        /// </summary>
        /// <value>The store URL.</value>
        public Uri StoreUrl { get; set; }
    }
}
