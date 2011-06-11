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
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <returns>
        /// The achievements that haven't been published yet.
        /// </returns>
        ICollection<SimpleAchievement> GetUnpublishedAchievements(string steamUserId, DateTime? oldestDate);

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>
        /// 	<see cref="Game"/>s for the givem steam user id.
        /// </returns>
        ICollection<Game> GetGames(string steamUserId);

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>The number of achievements that were updated.</returns>
        int UpdateAchievements(string steamUserId);

        /// <summary>
        /// Publishes the given user's achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The ids of the achievements to publish.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        bool PublishAchievements(string steamUserId, IEnumerable<int> achievementIds);

        /// <summary>
        /// Hides the given user's achievements
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The ids of the achievements to hide.</param>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        bool HideAchievements(string steamUserId, IEnumerable<int> achievementIds);

        /// <summary>
        /// Updates the new user's achievements and hides any that are more than 2 days old.
        /// </summary>
        /// <param name="user">The user.</param>
        void UpdateNewUserAchievements(User user);
    }
}