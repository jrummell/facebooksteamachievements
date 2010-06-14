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

using System.Collections.Generic;
using System.Linq;

namespace SteamAchievements.Data
{
    internal partial class SteamDataContext : ISteamRepository
    {
        #region ISteamRepository Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        IQueryable<Achievement> ISteamRepository.Achievements
        {
            get { return Achievements; }
        }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        IQueryable<UserAchievement> ISteamRepository.UserAchievements
        {
            get { return UserAchievements; }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        IQueryable<User> ISteamRepository.Users
        {
            get { return Users; }
        }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void InsertOnSubmit(User user)
        {
            Users.InsertOnSubmit(user);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            UserAchievements.DeleteAllOnSubmit(achievements);
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            Achievements.InsertOnSubmit(achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            UserAchievements.InsertAllOnSubmit(achievements);
        }

        #endregion
    }
}