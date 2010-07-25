#region License

// Copyright 2010 John Rummell
// 
// This file is part of SteamAchievements.
// 
//     SteamAchievements is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     SteamAchievements is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

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

        /// <summary>
        /// Gets or sets whether the game was played recently.
        /// </summary>
        public bool PlayedRecently { get; set; }
    }
}