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
using System.Data.Linq;
using System.Linq;

namespace SteamAchievements.Data
{
    public class SteamRepository : Disposable, ISteamRepository
    {
        private readonly SteamDataContext _context = new SteamDataContext();

        /// <summary>
        /// Gets the context.
        /// </summary>
        public DataContext Context
        {
            get { return _context; }
        }

        #region ISteamRepository Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        public IQueryable<Achievement> Achievements
        {
            get { return _context.Achievements; }
        }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        public IQueryable<UserAchievement> UserAchievements
        {
            get { return _context.UserAchievements; }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        public IQueryable<User> Users
        {
            get { return _context.Users; }
        }

        /// <summary>
        /// Gets the achievement names.
        /// </summary>
        /// <value>
        /// The achievement names.
        /// </value>
        public IQueryable<AchievementName> AchievementNames
        {
            get { return _context.AchievementNames; }
        }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void InsertOnSubmit(User user)
        {
            _context.Users.InsertOnSubmit(user);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _context.UserAchievements.DeleteAllOnSubmit(achievements);
        }

        /// <summary>
        /// Deletes the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void DeleteOnSubmit(User user)
        {
            _context.Users.DeleteOnSubmit(user);
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
            _context.SubmitChanges();
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            _context.Achievements.InsertOnSubmit(achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _context.UserAchievements.InsertAllOnSubmit(achievements);
        }

        /// <summary>
        /// Inserts the on submit.
        /// </summary>
        /// <param name="achievementName">Name of the achievement.</param>
        public void InsertOnSubmit(AchievementName achievementName)
        {
            _context.AchievementNames.InsertOnSubmit(achievementName);
        }

        #endregion

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManaged()
        {
            _context.Dispose();
        }
    }
}