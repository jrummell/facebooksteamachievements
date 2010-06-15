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
using SteamAchievements.Data.Properties;

namespace SteamAchievements.Data
{
    public class AchievementManager : IAchievementManager
    {
        private readonly ISteamRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementManager"/> class.
        /// </summary>
        public AchievementManager()
        {
            Type repositoryType = Type.GetType(Settings.Default.SteamRepositoryType);

            if (repositoryType == null)
            {
                string message =
                    String.Format(
                        "Could not create type instance from the SteamAchievements.Data.Properties.Settings.SteamRepositoryType value: {0}",
                        Settings.Default.SteamRepositoryType);
                throw new InvalidOperationException(message);
            }

            ISteamRepository instance = Activator.CreateInstance(repositoryType) as ISteamRepository;

            if (instance == null)
            {
                string message =
                    String.Format(
                        "Could not create an instance of {0} from the SteamAchievements.Data.Properties.Settings.SteamRepositoryType value: {1}",
                        typeof (ISteamRepository), Settings.Default.SteamRepositoryType);
                throw new InvalidOperationException(message);
            }

            _repository = instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementManager"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <remarks>
        /// This overload is for unit testing.
        /// </remarks>
        public AchievementManager(ISteamRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        #region IAchievementManager Members

        /// <summary>
        /// Gets the steam user id.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        public string GetSteamUserId(long facebookUserId)
        {
            IQueryable<string> query = from user in _repository.Users
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

            IQueryable<Achievement> achievements =
                from userAchievement in _repository.UserAchievements
                where userAchievement.SteamUserId == steamUserId
                      && userAchievement.Achievement.GameId == gameId
                orderby userAchievement.Date
                select userAchievement.Achievement;

            return achievements;
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievements">All achievements for the given user.</param>
        /// <remarks>
        /// Calls <see cref="InsertMissingAchievements"/> and <see cref="AssignAchievements"/>.
        /// </remarks>
        public int UpdateAchievements(string steamUserId, IEnumerable<Achievement> achievements)
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
                return 0;
            }

            IEnumerable<Achievement> missingAchievements = GetMissingAchievements(achievements);
            if (missingAchievements.Any())
            {
                InsertMissingAchievements(missingAchievements);
            }

            return AssignAchievements(steamUserId, achievements);
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

            DateTime fiveMinutesAgo = DateTime.Now.AddMinutes(-5);

            IQueryable<Achievement> achievements =
                from userAchievement in _repository.UserAchievements
                where userAchievement.SteamUserId == steamUserId && userAchievement.Date > fiveMinutesAgo
                orderby userAchievement.Date descending
                select userAchievement.Achievement;

            return achievements.Take(5);
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

            IQueryable<User> query = from u in _repository.Users
                                     where u.FacebookUserId == facebookUserId
                                     select u;

            User user = query.SingleOrDefault();

            if (user == null)
            {
                // the user does not exist, create a new one.
                user = new User {FacebookUserId = facebookUserId, SteamUserId = steamUserId};

                _repository.InsertOnSubmit(user);
                _repository.SubmitChanges();
            }
            else
            {
                // nothing to change
                if (user.SteamUserId == steamUserId)
                {
                    return;
                }

                // delete all achievements associated with the old id
                IQueryable<UserAchievement> userAchievements = from u in _repository.UserAchievements
                                                               where u.SteamUserId == user.SteamUserId
                                                               select u;

                _repository.DeleteAllOnSubmit(userAchievements.ToArray());
                _repository.SubmitChanges();

                // update steam id
                user.SteamUserId = steamUserId;
                _repository.SubmitChanges();
            }
        }

        public void Dispose()
        {
            _repository.Dispose();
        }

        #endregion

        /// <summary>
        /// Assigns the new achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievements">All achievements for the given user.</param>
        public int AssignAchievements(string steamUserId, IEnumerable<Achievement> achievements)
        {
            // get the achievement ids for the games in the given achievements
            IEnumerable<int> unassignedAchievementIds = GetUnassignedAchievementIds(steamUserId, achievements);

            if (!unassignedAchievementIds.Any())
            {
                return 0;
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

            _repository.InsertAllOnSubmit(newUserAchievements);
            _repository.SubmitChanges();

            return newUserAchievements.Count();
        }

        /// <summary>
        /// Gets the unassigned achievement ids.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="allAchievements">All achievements.</param>
        /// <returns></returns>
        public IEnumerable<int> GetUnassignedAchievementIds(string steamUserId, IEnumerable<Achievement> allAchievements)
        {
            IEnumerable<int> gameIds = (from a in allAchievements
                                        select a.GameId).Distinct();

            IEnumerable<string> achievementNames = from a in allAchievements
                                                   where gameIds.Contains(a.GameId)
                                                   select a.Name;

            IEnumerable<int> achievementIds = (from a in _repository.Achievements
                                               where achievementNames.Contains(a.Name)
                                               select a.Id).ToList();

            if (!achievementIds.Any())
            {
                return new int[0];
            }

            // get the unassigned achievement ids
            IEnumerable<int> assignedAchievementIds = (from a in _repository.UserAchievements
                                                       where a.SteamUserId == steamUserId
                                                       select a.AchievementId).ToList();

            return from id in achievementIds
                   where !assignedAchievementIds.Contains(id)
                   select id;
        }

        /// <summary>
        /// Inserts the missing achievements.
        /// </summary>
        /// <param name="missingAchievements">The missing achievements.</param>
        public void InsertMissingAchievements(IEnumerable<Achievement> missingAchievements)
        {
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
                _repository.InsertOnSubmit(newAchievement);
            }

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Gets the missing database achievements.
        /// </summary>
        /// <param name="communityAchievements">The community achievements.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetMissingAchievements(IEnumerable<Achievement> communityAchievements)
        {
            return from achievement in communityAchievements
                   where
                       !_repository.Achievements.Any(
                            dbAchievement =>
                            dbAchievement.GameId == achievement.GameId &&
                            dbAchievement.Name.ToUpper() == achievement.Name.ToUpper())
                   select achievement;
        }

        /// <summary>
        /// Adds achievements for a game.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <param name="achievements">The game achievements.</param>
        public void AddAchievements(int gameId, IEnumerable<Achievement> achievements)
        {
            if (achievements == null)
            {
                throw new ArgumentNullException("achievements");
            }

            foreach (Achievement achievement in achievements)
            {
                achievement.GameId = gameId;

                string name = achievement.Name;
                if (!_repository.Achievements.Where(a => a.GameId == gameId && a.Name.ToUpper() == name.ToUpper()).Any())
                {
                    _repository.InsertOnSubmit(achievement);
                }
            }

            _repository.SubmitChanges();
        }
    }
}