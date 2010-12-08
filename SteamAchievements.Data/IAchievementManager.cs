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

namespace SteamAchievements.Data
{
    public interface IAchievementManager : IDisposable
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="steamUserId">The facebook user id.</param>
        /// <returns></returns>
        User GetUser(long facebookUserId);

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="gameId">The game id.</param>
        /// <returns></returns>
        IEnumerable<Achievement> GetAchievements(string steamUserId, int gameId);

        /// <summary>
        /// Gets the unpublished achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        IEnumerable<Achievement> GetUnpublishedAchievements(string steamUserId);

        /// <summary>
        /// Gets the auto update users.
        /// </summary>
        /// <returns></returns>
        IEnumerable<User> GetAutoUpdateUsers();

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        int UpdateAchievements(IEnumerable<UserAchievement> achievements);

        /// <summary>
        /// Updates the published flag on the given achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        void UpdatePublished(string steamUserId, IEnumerable<int> achievementIds);

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="user"></param>
        void UpdateUser(User user);
    }
}