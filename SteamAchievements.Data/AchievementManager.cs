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
    public class AchievementManager
    {
        /// <summary>
        /// Gets the steam user id.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        public string GetSteamUserId(long facebookUserId)
        {
            SteamDataContext context = new SteamDataContext();

            IQueryable<string> query = from user in context.Users
                                       where user.FacebookUserId == facebookUserId
                                       select user.SteamUserId;

            return query.SingleOrDefault();
        }

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="gameId">The game id.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetAchievements(string steamUserId, int gameId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            SteamDataContext context = new SteamDataContext();
            IQueryable<Achievement> achievements =
                from userAchievement in context.UserAchievements
                where userAchievement.SteamUserId == steamUserId && userAchievement.Achievement.GameId == gameId
                select userAchievement.Achievement;

            return achievements;
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievements">All achievements for the given user.</param>
        public void UpdateAchievements(string steamUserId, IEnumerable<Achievement> achievements)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (achievements == null)
            {
                throw new ArgumentNullException("achievements");
            }

            if (!achievements.Any())
            {
                return;
            }

            InsertMissingAchievements(achievements);
            AssignNewAchievements(steamUserId, achievements);
        }

        /// <summary>
        /// Gets the last 5 achievements added in the last 5 minutes.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetLatestAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            DateTime oneHourAgo = DateTime.Now.AddMinutes(-5);

            SteamDataContext context = new SteamDataContext();
            IQueryable<Achievement> achievements =
                from userAchievement in context.UserAchievements
                where userAchievement.SteamUserId == steamUserId && userAchievement.Date > oneHourAgo
                orderby userAchievement.Date descending
                select userAchievement.Achievement;

            return achievements.Take(5);
        }

        /// <summary>
        /// Assigns the new achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievements">All achievements for the given user.</param>
        private void AssignNewAchievements(string steamUserId, IEnumerable<Achievement> achievements)
        {
            // get the achievement ids for the games in the given achievements
            IEnumerable<int> gameIds = (from a in achievements
                                        select a.GameId).Distinct();

            IEnumerable<string> achievementNames = from a in achievements
                                                   where gameIds.Contains(a.GameId)
                                                   select a.Name;

            SteamDataContext context = new SteamDataContext {ObjectTrackingEnabled = false};
            IEnumerable<int> achievementIds = (from a in context.Achievements
                                               where achievementNames.Contains(a.Name)
                                               select a.Id).ToList();

            if (!achievementIds.Any())
            {
                return;
            }

            // get the unassigned achievement ids
            IEnumerable<int> assignedAchievementIds = (from a in context.UserAchievements
                                                       where a.SteamUserId == steamUserId
                                                       select a.AchievementId).ToList();

            IEnumerable<int> unassignedAchievementIds = from id in achievementIds
                                                        where !assignedAchievementIds.Contains(id)
                                                        select id;

            if (!unassignedAchievementIds.Any())
            {
                return;
            }

            // create the unassigned achievements and insert them
            IEnumerable<UserAchievement> newUserAchievements =
                from id in unassignedAchievementIds
                select new UserAchievement
                           {
                               SteamUserId = steamUserId,
                               AchievementId = id,
                               Date = DateTime.Now
                           };

            SteamDataContext context2 = new SteamDataContext();
            context2.UserAchievements.InsertAllOnSubmit(newUserAchievements);
            context2.SubmitChanges();
        }

        /// <summary>
        /// Inserts the missing achievements.
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        private void InsertMissingAchievements(IEnumerable<Achievement> achievements)
        {
            SteamDataContext context = new SteamDataContext();

            IEnumerable<Achievement> dbAchievements = context.Achievements.ToList();

            IEnumerable<Achievement> missingAchievements = from achievement in achievements
                                                           join dbAchievement in dbAchievements on
                                                               achievement.Name equals
                                                               dbAchievement.Name into x
                                                           from missing in x.DefaultIfEmpty()
                                                           where missing == null
                                                           select achievement;

            if (!missingAchievements.Any())
            {
                return;
            }

            foreach (Achievement achievement in missingAchievements)
            {
                Achievement newAchievement = new Achievement
                                                 {
                                                     Name = achievement.Name,
                                                     GameId = achievement.GameId,
                                                     Description = achievement.Description,
                                                     ImageUrl = achievement.ImageUrl
                                                 };
                context.Achievements.InsertOnSubmit(newAchievement);
            }

            context.SubmitChanges();
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Game> GetGames()
        {
            SteamDataContext context = new SteamDataContext();
            return from game in context.Games
                   orderby game.Name
                   select game;
        }

        /// <summary>
        /// Updates the steam user id.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="steamUserId">The steam user id.</param>
        public void UpdateSteamUserId(long facebookUserId, string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            SteamDataContext context = new SteamDataContext();
            IQueryable<User> query = from u in context.Users
                                     where u.FacebookUserId == facebookUserId
                                     select u;

            User user = query.SingleOrDefault();

            if (user == null)
            {
                // the user does not exist, create a new one.
                user = new User {FacebookUserId = facebookUserId, SteamUserId = steamUserId};

                context.Users.InsertOnSubmit(user);
            }
            else
            {
                // update steam id
                string oldSteamUserId = user.SteamUserId;
                user.SteamUserId = steamUserId;

                // delete all achievements associated with the old id
                IQueryable<UserAchievement> users = from u in context.UserAchievements
                                                    where u.SteamUserId == oldSteamUserId
                                                    select u;

                context.UserAchievements.DeleteAllOnSubmit(users);
            }

            context.SubmitChanges();
        }
    }
}