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
                        typeof(ISteamRepository), Settings.Default.SteamRepositoryType);
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
        /// Gets the user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        public User GetUser(long facebookUserId)
        {
            IQueryable<User> query = from user in _repository.Users
                                     where user.FacebookUserId == facebookUserId
                                     select user;

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
        /// <param name="achievements">All achievements for the given user.</param>
        /// <remarks>
        /// Calls <see cref="InsertMissingAchievements"/> and <see cref="AssignAchievements"/>.
        /// </remarks>
        public int UpdateAchievements(IEnumerable<UserAchievement> achievements)
        {
            if (achievements == null)
            {
                throw new ArgumentNullException("achievements");
            }

            if (!achievements.Any())
            {
                return 0;
            }

            string steamUserId = achievements.First().SteamUserId;
            if (achievements.Any(achievement => achievement.SteamUserId != steamUserId))
            {
                throw new ArgumentException("All achievements must have the same SteamUserId", "achievements");
            }

            IEnumerable<Achievement> missingAchievements =
                GetMissingAchievements(achievements.Select(a => a.Achievement));
            if (missingAchievements.Any())
            {
                InsertMissingAchievements(missingAchievements);
            }

            return AssignAchievements(achievements);
        }

        /// <summary>
        /// Updates the user. Passing a new user with FacebookUserId and SteamUserId will insert a new User.
        /// </summary>
        /// <param name="user">The user.</param>
        public void UpdateUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (String.IsNullOrEmpty(user.SteamUserId))
            {
                throw new ArgumentException("user.SteamUserId cannot be null or empty", "user");
            }

            if (user.FacebookUserId == 0)
            {
                throw new ArgumentException("user.FacebookUserId cannot be 0", "user");
            }

            bool exists = _repository.Users.Where(u => u.FacebookUserId == user.FacebookUserId).Any();
            
            if (!exists)
            {
                // the user does not exist, create a new one.
                User newUser = new User
                        {
                            FacebookUserId = user.FacebookUserId,
                            SteamUserId = user.SteamUserId,
                            AccessToken = user.AccessToken,
                            AutoUpdate = user.AutoUpdate
                        };

                _repository.InsertOnSubmit(newUser);
                _repository.SubmitChanges();

                return;
            }

            User existingUser = _repository.Users.Where(u => u.FacebookUserId == user.FacebookUserId).Single();

            string existingSteamUserId = existingUser.SteamUserId;
            bool steamIdChanged = existingSteamUserId.ToUpper() != user.SteamUserId.ToUpper();
            bool hasAchievements = false;
            List<UserAchievement> userAchievements = null;
            if (steamIdChanged)
            {
                // delete the user's acheivements
                userAchievements = (from u in _repository.UserAchievements
                                    where u.SteamUserId.ToUpper() == existingSteamUserId.ToUpper()
                                    select u).ToList();

                hasAchievements = userAchievements.Any();
                if (hasAchievements)
                {
                    _repository.DeleteAllOnSubmit(userAchievements);
                    _repository.SubmitChanges();
                }
            }

            // update
            existingUser = _repository.Users.Where(u => u.FacebookUserId == user.FacebookUserId).Single();
            existingUser.AccessToken = user.AccessToken;
            existingUser.AutoUpdate = user.AutoUpdate;
            existingUser.SteamUserId = user.SteamUserId;

            _repository.SubmitChanges();

            if (steamIdChanged && hasAchievements)
            {
                // insert the user's updated achievements
                List<UserAchievement> updatedUserAchievements = new List<UserAchievement>();

                foreach (UserAchievement userAchievement in userAchievements)
                {
                    UserAchievement updated =
                        new UserAchievement
                            {
                                AchievementId = userAchievement.AchievementId,
                                Date = userAchievement.Date,
                                Id = userAchievement.Id,
                                Published = userAchievement.Published,
                                SteamUserId = user.SteamUserId
                            };

                    updatedUserAchievements.Add(updated);
                }

                _repository.InsertAllOnSubmit(updatedUserAchievements);
                _repository.SubmitChanges();
            }
        }

        /// <summary>
        /// Updates the published flag of the given achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        public void UpdatePublished(string steamUserId, IEnumerable<int> achievementIds)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (achievementIds == null)
            {
                throw new ArgumentNullException("achievementIds");
            }

            if (!achievementIds.Any())
            {
                return;
            }

            IQueryable<UserAchievement> achievementsToUpdate =
                from achievement in _repository.UserAchievements
                where achievement.SteamUserId == steamUserId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            foreach (UserAchievement achievement in achievementsToUpdate)
            {
                achievement.Published = true;
            }

            _repository.SubmitChanges();
        }

        public IEnumerable<Achievement> GetUnpublishedAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            return from achievement in _repository.UserAchievements
                   where achievement.SteamUserId == steamUserId && !achievement.Published
                   select achievement.Achievement;
        }

        public IEnumerable<User> GetAutoUpdateUsers()
        {
            return _repository.Users.Where(user => user.AutoUpdate).ToList();
        }

        #endregion

        /// <summary>
        /// Assigns the new achievements.
        /// </summary>
        /// <param name="achievements">All achievements for the given user.</param>
        /// <returns></returns>
        public int AssignAchievements(IEnumerable<UserAchievement> achievements)
        {
            if (achievements == null)
            {
                throw new ArgumentNullException("achievements");
            }

            if (!achievements.Any())
            {
                return 0;
            }

            // get the achievement ids for the games in the given achievements
            string steamUserId = achievements.First().SteamUserId;
            IEnumerable<Achievement> unassignedAchievements =
                GetUnassignedAchievements(steamUserId, achievements.Select(achievement => achievement.Achievement));

            if (!unassignedAchievements.Any())
            {
                return 0;
            }

            // create the unassigned achievements and insert them
            IEnumerable<UserAchievement> newUserAchievements =
                from achievement in unassignedAchievements
                select new UserAchievement
                           {
                               SteamUserId = steamUserId,
                               AchievementId = achievement.Id,
                               Date = DateTime.Now //TODO: use achievement date
                           };

            _repository.InsertAllOnSubmit(newUserAchievements);
            _repository.SubmitChanges();

            return newUserAchievements.Count();
        }

        /// <summary>
        /// Gets the unassigned achievement ids.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="allAchievements">All achievements. These will not necessarily have an Id set.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> GetUnassignedAchievements(string steamUserId,
                                                                  IEnumerable<Achievement> allAchievements)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (allAchievements == null)
            {
                throw new ArgumentNullException("allAchievements");
            }

            if (!allAchievements.Any())
            {
                return new Achievement[0];
            }

            IEnumerable<int> gameIds = (from a in allAchievements
                                        select a.GameId).Distinct();

            IEnumerable<string> achievementNames = (from a in allAchievements
                                                    where gameIds.Contains(a.GameId)
                                                    select a.Name).Distinct();

            // get the possible achievements by name
            IEnumerable<Achievement> possibleAchievements = (from a in _repository.Achievements
                                                             where achievementNames.Contains(a.Name)
                                                             select a).ToList();

            if (!possibleAchievements.Any())
            {
                return new Achievement[0];
            }

            // get all assigned achievements
            IEnumerable<Achievement> assignedAchievements = (from a in _repository.UserAchievements
                                                             where a.SteamUserId == steamUserId
                                                             select a.Achievement).ToList();

            // return the unassigned achievements. I'm not hitting the database at this point since that could 
            // add a great deal of complexity to the following query.
            return from achievement in allAchievements
                   join possibleAchievement in possibleAchievements
                       on new { achievement.GameId, achievement.Name, achievement.Description }
                       equals
                       new { possibleAchievement.GameId, possibleAchievement.Name, possibleAchievement.Description }
                   join assignedAchievement in assignedAchievements
                       on possibleAchievement.Id equals assignedAchievement.Id into joinedAssignedAchievements
                   from joinedAssignedAchievement in joinedAssignedAchievements.DefaultIfEmpty()
                   where joinedAssignedAchievement == null
                   select possibleAchievement;
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    _repository.Dispose();
                }
            }
        }
    }
}