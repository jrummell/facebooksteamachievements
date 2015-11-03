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
using System.Linq;

namespace SteamAchievements.Data
{
    public interface ISteamRepository : IDisposable
    {
        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        IQueryable<steam_Achievement> Achievements { get; }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        IQueryable<steam_UserAchievement> UserAchievements { get; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        IQueryable<steam_User> Users { get; }

        /// <summary>
        /// Gets the achievement names.
        /// </summary>
        /// <value>
        /// The achievement names.
        /// </value>
        IQueryable<steam_AchievementName> AchievementNames { get; }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        void InsertOnSubmit(steam_User user);

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        void DeleteAllOnSubmit(IEnumerable<steam_UserAchievement> achievements);

        /// <summary>
        /// Deletes the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        void DeleteOnSubmit(steam_User user);

        /// <summary>
        /// Submits the changes.
        /// </summary>
        void SubmitChanges();

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        void InsertOnSubmit(steam_Achievement achievement);

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        void InsertAllOnSubmit(IEnumerable<steam_UserAchievement> achievements);

        /// <summary>
        /// Inserts the on submit.
        /// </summary>
        /// <param name="achievementName">Name of the achievement.</param>
        void InsertOnSubmit(steam_AchievementName achievementName);
    }
}