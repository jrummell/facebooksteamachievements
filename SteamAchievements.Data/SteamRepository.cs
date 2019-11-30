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
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace SteamAchievements.Data
{
    public class SteamRepository : Disposable, ISteamRepository
    {
        private readonly DbContext _context;

        public SteamRepository(DbContext context)
        {
            _context = context;
        }

        #region ISteamRepository Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <value>The achievements.</value>
        public IQueryable<Achievement> Achievements
        {
            get { return _context.Set<Achievement>(); }
        }

        /// <summary>
        /// Gets the user achievements.
        /// </summary>
        /// <value>The user achievements.</value>
        public IQueryable<UserAchievement> UserAchievements
        {
            get { return _context.Set<UserAchievement>(); }
        }

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        public IQueryable<User> Users
        {
            get { return _context.Set<User>(); }
        }

        /// <summary>
        /// Gets the achievement names.
        /// </summary>
        /// <value>
        /// The achievement names.
        /// </value>
        public IQueryable<AchievementName> AchievementNames
        {
            get { return _context.Set<AchievementName>(); }
        }

        /// <summary>
        /// Inserts the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void InsertOnSubmit(User user)
        {
            _context.Set<User>().Add(user);
        }

        /// <summary>
        /// Deletes all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void DeleteAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _context.Set<UserAchievement>().RemoveRange(achievements);
        }

        /// <summary>
        /// Deletes the user on submit.
        /// </summary>
        /// <param name="user">The user.</param>
        public void DeleteOnSubmit(User user)
        {
            _context.Set<User>().Remove(user);
        }

        /// <summary>
        /// Submits the changes.
        /// </summary>
        public void SubmitChanges()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Inserts the achievement on submit.
        /// </summary>
        /// <param name="achievement">The achievement.</param>
        public void InsertOnSubmit(Achievement achievement)
        {
            _context.Set<Achievement>().Add(achievement);
        }

        /// <summary>
        /// Inserts all given achievements on submit.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        public void InsertAllOnSubmit(IEnumerable<UserAchievement> achievements)
        {
            _context.Set<UserAchievement>().AddRange(achievements);
        }

        /// <summary>
        /// Inserts the on submit.
        /// </summary>
        /// <param name="achievementName">Name of the achievement.</param>
        public void InsertOnSubmit(AchievementName achievementName)
        {
            _context.Set<AchievementName>().Add(achievementName);
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