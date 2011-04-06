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
    [Serializable]
    public class FacebookSettings
    {
        /// <summary>
        /// Gets or sets the facebook user id.
        /// </summary>
        /// <value>
        /// The facebook user id.
        /// </value>
        public long FacebookUserId { get; set; }

        /// <summary>
        /// Gets or sets the steam user id.
        /// </summary>
        /// <value>
        /// The steam user id.
        /// </value>
        public string SteamUserId { get; set; }

        /// <summary>
        /// Gets or sets the access token.
        /// </summary>
        /// <value>
        /// The access token.
        /// </value>
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to publish the description.
        /// </summary>
        /// <value>
        ///   <c>true</c> if publish the description; otherwise, <c>false</c>.
        /// </value>
        public bool PublishDescription { get; set; }
    }
}