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
        IQueryable<Achievement> Achievements { get; }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        IQueryable<UserAchievement> UserAchievements { get; }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        IQueryable<User> Users { get; }

        /// <summary>
        /// Gets the achievement names.
        /// </summary>
        /// <value>
        /// The achievement names.
        /// </value>
        IQueryable<AchievementName> AchievementNames { get; }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        void InsertOnSubmit(User user);

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements);

        /// <summary>
        /// Deletes the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        void DeleteOnSubmit(User user);

        /// <summary>
        /// Submits the changes.
        /// </summary>
        void SubmitChanges();

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        void InsertOnSubmit(Achievement achievement);

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements);

        /// <summary>
        /// Inserts the on submit.
        /// </summary>
        /// <param name="achievementName">Name of the achievement.</param>
        void InsertOnSubmit(AchievementName achievementName);
    }
}