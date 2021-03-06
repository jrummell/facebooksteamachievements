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
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public interface IAchievementManager : IDisposable
    {
        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        User GetUser(string userId);

        /// <summary>
        /// Gets the unpublished achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns></returns>
        ICollection<Achievement> GetUnpublishedAchievements(string userId);

        /// <summary>
        /// Gets the unpublished achievements by oldest date.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <returns></returns>
        ICollection<Achievement> GetUnpublishedAchievements(string userId, DateTime oldestDate);

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        int UpdateAchievements(IEnumerable<UserAchievement> achievements);

        /// <summary>
        /// Updates the published flag on the given achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        void UpdatePublished(string userId, IEnumerable<int> achievementIds);

        /// <summary>
        /// Updates the hidden flag on the given achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        void UpdateHidden(string userId, IEnumerable<int> achievementIds);
    }
}