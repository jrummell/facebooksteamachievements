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
using System.Data.SqlTypes;
using System.Linq;

namespace SteamAchievements.Data
{
    public class AchievementManager : IAchievementManager
    {
        private readonly ISteamRepository _repository;

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
        /// Gets the user.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public User GetUser(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            IQueryable<User> query = from user in _repository.Users
                                     where user.SteamUserId == steamUserId
                                     select user;

            return query.SingleOrDefault();
        }

        /// <summary>
        /// Gets the auto update users.
        /// </summary>
        /// <returns></returns>
        public ICollection<User> GetAutoUpdateUsers()
        {
            return (from user in _repository.Users
                    where user.AutoUpdate && user.AccessToken != null && user.AccessToken.Length > 0
                    orderby user.SteamUserId
                    select user).ToList();
        }

        /// <summary>
        /// Gets the unpublished achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        public ICollection<Achievement> GetUnpublishedAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            long facebookUserId = GetFacebookUserId(steamUserId);

            return (from achievement in _repository.UserAchievements
                    where achievement.FacebookUserId == facebookUserId
                          && !achievement.Published
                          && !achievement.Hidden
                    orderby achievement.Date descending
                    select achievement.Achievement).ToList();
        }

        /// <summary>
        /// Gets the unpublished achievements by date.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <returns></returns>
        public ICollection<Achievement> GetUnpublishedAchievements(string steamUserId, DateTime oldestDate)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            oldestDate = ValidateDate(oldestDate);

            long facebookUserId = GetFacebookUserId(steamUserId);

            return (from achievement in _repository.UserAchievements
                    where achievement.FacebookUserId == facebookUserId
                          && !achievement.Published
                          && !achievement.Hidden
                          && achievement.Date > oldestDate
                    orderby achievement.Date descending
                    select achievement.Achievement).ToList();
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

            long facebookUserId = achievements.First().FacebookUserId;
            if (achievements.Any(achievement => achievement.FacebookUserId != facebookUserId))
            {
                throw new ArgumentException("All achievements must have the same SteamUserId", "achievements");
            }

            ICollection<Achievement> missingAchievements =
                GetMissingAchievements(achievements.Select(a => a.Achievement).ToList());
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

            bool exists = Exists(user.FacebookUserId);
            if (!exists)
            {
                // the user does not exist, create a new one.
                User newUser = new User
                                   {
                                       FacebookUserId = user.FacebookUserId,
                                       SteamUserId = user.SteamUserId,
                                       AccessToken = user.AccessToken,
                                       AutoUpdate = user.AutoUpdate,
                                       PublishDescription = user.PublishDescription
                                   };

                _repository.InsertOnSubmit(newUser);
                _repository.SubmitChanges();

                return;
            }

            // update
            User existingUser = _repository.Users.Where(u => u.FacebookUserId == user.FacebookUserId).Single();
            existingUser.AccessToken = user.AccessToken;
            existingUser.AutoUpdate = user.AutoUpdate;
            existingUser.SteamUserId = user.SteamUserId;
            existingUser.PublishDescription = user.PublishDescription;

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Deauthorizes the user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        public void DeauthorizeUser(long facebookUserId)
        {
            User user = _repository.Users.Where(u => u.FacebookUserId == facebookUserId).SingleOrDefault();
            if (user == null)
            {
                return;
            }

            IQueryable<UserAchievement> userAchievements =
                _repository.UserAchievements.Where(ua => ua.FacebookUserId == facebookUserId);
            _repository.DeleteAllOnSubmit(userAchievements);
            _repository.SubmitChanges();

            _repository.DeleteOnSubmit(user);
            _repository.SubmitChanges();
        }

        /// <summary>
        /// Determines whether the specified user is a duplicate.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns>
        ///   <c>true</c> if the specified user is a duplicate; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDuplicate(string steamUserId, long facebookUserId)
        {
            bool exists = Exists(facebookUserId);

            return IsDuplicate(steamUserId, facebookUserId, exists);
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

            long facebookUserId = GetFacebookUserId(steamUserId);

            IQueryable<UserAchievement> achievementsToUpdate =
                from achievement in _repository.UserAchievements
                where achievement.FacebookUserId == facebookUserId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            foreach (UserAchievement achievement in achievementsToUpdate)
            {
                achievement.Published = true;
            }

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Updates the hidden flag on the given achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        public void UpdateHidden(string steamUserId, IEnumerable<int> achievementIds)
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

            long facebookUserId = GetFacebookUserId(steamUserId);

            //Note: achievementIds.Contains() will only translate to SQL if achievementIds is IEnumerable<int> (not ICollection<int>).
            IQueryable<UserAchievement> achievementsToUpdate =
                from achievement in _repository.UserAchievements
                where achievement.FacebookUserId == facebookUserId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            foreach (UserAchievement achievement in achievementsToUpdate)
            {
                achievement.Hidden = true;
            }

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Existses the specified facebook user id.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        private bool Exists(long facebookUserId)
        {
            return _repository.Users.Where(u => u.FacebookUserId == facebookUserId).Any();
        }

        /// <summary>
        /// Determines whether the specified steam user id is duplicate.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="exists">if set to <c>true</c> [exists].</param>
        /// <returns>
        ///   <c>true</c> if the specified steam user id is duplicate; otherwise, <c>false</c>.
        /// </returns>
        private bool IsDuplicate(string steamUserId, long facebookUserId, bool exists)
        {
            bool duplicate;
            if (exists)
            {
                duplicate =
                    _repository.Users.Where(
                        u => u.SteamUserId == steamUserId && u.FacebookUserId != facebookUserId).Any();
            }
            else
            {
                duplicate = _repository.Users.Where(u => u.SteamUserId == steamUserId).Any();
            }

            return duplicate;
        }

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
            long facebookUserId = achievements.First().FacebookUserId;
            IEnumerable<Achievement> unassignedAchievements =
                GetUnassignedAchievements(facebookUserId, achievements.Select(achievement => achievement.Achievement));

            if (!unassignedAchievements.Any())
            {
                return 0;
            }

            // create the unassigned achievements and insert them
            IEnumerable<UserAchievement> newUserAchievements =
                from achievement in unassignedAchievements
                join userAchievement in achievements on
                    new
                        {
                            achievement.ApiName,
                            achievement.GameId
                        } equals
                    new
                        {
                            userAchievement.Achievement.ApiName,
                            userAchievement.Achievement.GameId
                        }
                select new UserAchievement
                           {
                               FacebookUserId = facebookUserId,
                               AchievementId = achievement.Id,
                               Date = ValidateDate(userAchievement.Date),
                               Published = false
                           };

            _repository.InsertAllOnSubmit(newUserAchievements);
            _repository.SubmitChanges();

            return newUserAchievements.Count();
        }

        /// <summary>
        /// Validates the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static DateTime ValidateDate(DateTime date)
        {
            DateTime minValue = new DateTime(SqlDateTime.MinValue.Value.Ticks);
            DateTime maxValue = new DateTime(SqlDateTime.MaxValue.Value.Ticks);
            if (date > minValue && date < maxValue)
            {
                return date;
            }

            return DateTime.Now;
        }

        /// <summary>
        /// Gets the unassigned achievement ids.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="allAchievements">All achievements. These will not necessarily have an Id set.</param>
        /// <returns></returns>
        public ICollection<Achievement> GetUnassignedAchievements(long facebookUserId,
                                                                  IEnumerable<Achievement> allAchievements)
        {
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

            IEnumerable<string> achievementApiNames = (from a in allAchievements
                                                       where gameIds.Contains(a.GameId)
                                                       select a.ApiName).Distinct();

            // get the possible achievements by name
            IEnumerable<Achievement> possibleAchievements = (from a in _repository.Achievements
                                                             where achievementApiNames.Contains(a.ApiName)
                                                             select a).ToList();

            if (!possibleAchievements.Any())
            {
                return new Achievement[0];
            }

            // get all assigned achievements
            IEnumerable<Achievement> assignedAchievements = (from a in _repository.UserAchievements
                                                             where a.FacebookUserId == facebookUserId
                                                             select a.Achievement).ToList();

            // return the unassigned achievements. I'm not hitting the database at this point since that could 
            // add a great deal of complexity to the following query.
            return (from achievement in allAchievements
                    join possibleAchievement in possibleAchievements
                        on new {achievement.GameId, achievement.ApiName}
                        equals
                        new {possibleAchievement.GameId, possibleAchievement.ApiName}
                    join assignedAchievement in assignedAchievements
                        on possibleAchievement.Id equals assignedAchievement.Id into joinedAssignedAchievements
                    from joinedAssignedAchievement in joinedAssignedAchievements.DefaultIfEmpty()
                    where joinedAssignedAchievement == null
                    select possibleAchievement).ToList();
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
                                                     ApiName = achievement.ApiName,
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
        public ICollection<Achievement> GetMissingAchievements(IEnumerable<Achievement> communityAchievements)
        {
            if (communityAchievements == null)
            {
                throw new ArgumentNullException("communityAchievements");
            }

            if (!communityAchievements.Any())
            {
                return new Achievement[0];
            }

            IEnumerable<string> communityAchievementIds =
                from achievement in communityAchievements
                select achievement.GameId + achievement.ApiName;

            List<Achievement> dbAchievements =
                (from achievement in _repository.Achievements
                 let achievementId = achievement.GameId + achievement.ApiName
                 where communityAchievementIds.Contains(achievementId)
                 select achievement).ToList();

            List<Achievement> missingAchievements = new List<Achievement>();
            if (communityAchievements.Count() != dbAchievements.Count)
            {
                foreach (Achievement achievement in communityAchievements)
                {
                    Achievement communityAchievement = achievement;
                    Achievement missingAchievement =
                        dbAchievements.Where(
                            a => a.GameId == communityAchievement.GameId
                                 && a.ApiName == communityAchievement.ApiName)
                            .FirstOrDefault();

                    if (missingAchievement == null)
                    {
                        missingAchievements.Add(communityAchievement);
                    }
                }
            }

            return missingAchievements;
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

                string apiName = achievement.ApiName;
                if (!_repository.Achievements.Where(a => a.GameId == gameId && a.ApiName == apiName).Any())
                {
                    _repository.InsertOnSubmit(achievement);
                }
            }

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Gets the facebook user id.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        private long GetFacebookUserId(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new NullReferenceException("steamUserId");
            }

            return (from user in _repository.Users
                    where user.SteamUserId == steamUserId
                    select user.FacebookUserId).SingleOrDefault();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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