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
using System.Collections.Generic;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public interface IAchievementService : IDisposable
    {
        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        SteamProfile GetProfile(string steamUserId);

        /// <summary>
        /// Gets the unpublished achievements newer than the given date.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The achievements that haven't been published yet.
        /// </returns>
        ICollection<Achievement> GetUnpublishedAchievements(long facebookUserId, DateTime? oldestDate, string language = null);

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns>
        ///   <see cref="Game"/>s for the givem steam user id.
        /// </returns>
        ICollection<Game> GetGames(long facebookUserId);

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The number of achievements that were updated.
        /// </returns>
        int UpdateAchievements(long facebookUserId, string language = null);

        /// <summary>
        /// Publishes the given user's achievements.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="achievementIds">The ids of the achievements to publish.</param>
        /// <returns>
        /// true if successful, else false.
        /// </returns>
        bool PublishAchievements(long facebookUserId, IEnumerable<int> achievementIds);

        /// <summary>
        /// Hides the given user's achievements
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="achievementIds">The ids of the achievements to hide.</param>
        /// <returns></returns>
        bool HideAchievements(long facebookUserId, IEnumerable<int> achievementIds);

        /// <summary>
        /// Updates the new user's achievements and hides any that are more than 2 days old.
        /// </summary>
        /// <param name="user">The user.</param>
        void UpdateNewUserAchievements(User user);
    }
}