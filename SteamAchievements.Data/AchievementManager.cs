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
    public class AchievementManager : Disposable, IAchievementManager
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
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns></returns>
        public ICollection<Achievement> GetUnpublishedAchievements(long facebookUserId)
        {
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
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <returns></returns>
        public ICollection<Achievement> GetUnpublishedAchievements(long facebookUserId, DateTime oldestDate)
        {
            oldestDate = ValidateDate(oldestDate);

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
        /// <param name="userAchievements">All achievements for the given user.</param>
        /// <returns></returns>
        /// <remarks>
        /// Calls <see cref="InsertMissingAchievements"/> and <see cref="AssignAchievements"/>.
        /// </remarks>
        public int UpdateAchievements(IEnumerable<UserAchievement> userAchievements)
        {
            if (userAchievements == null)
            {
                throw new ArgumentNullException("userAchievements");
            }

            if (!userAchievements.Any())
            {
                return 0;
            }

            long facebookUserId = userAchievements.First().FacebookUserId;
            if (userAchievements.Any(achievement => achievement.FacebookUserId != facebookUserId))
            {
                throw new ArgumentException("All achievements must have the same SteamUserId", "userAchievements");
            }

            List<Achievement> achievements = userAchievements.Select(a => a.Achievement).ToList();
            ICollection<Achievement> missingAchievements = GetMissingAchievements(achievements);
            if (missingAchievements.Any())
            {
                InsertMissingAchievements(missingAchievements);
            }

            ICollection<AchievementName> missingAchievementNames =
                GetMissingAchievementNames(userAchievements.Select(a => a.Achievement).ToList());
            if (missingAchievementNames.Any())
            {
                InsertMissingAchievementNames(missingAchievementNames);
            }

            return AssignAchievements(userAchievements);
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
            
            // if the user changed steam IDs, remove their achievements
            if (existingUser.SteamUserId != user.SteamUserId)
            {
            	var existingAchievements = _repository.UserAchievements
            		.Where(ua => ua.FacebookUserId == user.FacebookUserId)
            		.ToArray();
            	if (existingAchievements.Length > 0)
            	{
            		_repository.DeleteAllOnSubmit(existingAchievements);
            	}
            }
            
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
        /// Updates the published flag of the given achievements.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        public void UpdatePublished(long facebookUserId, IEnumerable<int> achievementIds)
        {
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
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="achievementIds">The achievement ids.</param>
        public void UpdateHidden(long facebookUserId, IEnumerable<int> achievementIds)
        {
            if (achievementIds == null)
            {
                throw new ArgumentNullException("achievementIds");
            }

            if (!achievementIds.Any())
            {
                return;
            }

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

        #endregion

        protected override void DisposeManaged()
        {
            _repository.Dispose();
        }

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
        public void InsertMissingAchievements(ICollection<Achievement> missingAchievements)
        {
            if (missingAchievements == null)
            {
                throw new ArgumentNullException("missingAchievements");
            }

            if (missingAchievements.Count == 0)
            {
                return;
            }

            foreach (Achievement achievement in missingAchievements)
            {
                Achievement newAchievement = new Achievement
                                                 {
                                                     ApiName = achievement.ApiName,
                                                     GameId = achievement.GameId,
                                                     ImageUrl = achievement.ImageUrl
                                                 };
                AchievementName name = achievement.AchievementNames.First();
                AchievementName newAchievementName = new AchievementName
                                                         {
                                                             Language = name.Language,
                                                             Name = name.Name,
                                                             Description = name.Description
                                                         };
                newAchievement.AchievementNames.Add(newAchievementName);
                _repository.InsertOnSubmit(newAchievement);
            }

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Inserts the missing achievement names.
        /// </summary>
        /// <param name="missingAchievementNames">The missing achievement names.</param>
        /// <returns></returns>
        private void InsertMissingAchievementNames(ICollection<AchievementName> missingAchievementNames)
        {
            if (missingAchievementNames == null)
            {
                throw new ArgumentNullException("missingAchievementNames");
            }

            if (missingAchievementNames.Count == 0)
            {
                return;
            }

            foreach (AchievementName achievementName in missingAchievementNames)
            {
                AchievementName newAchievementName = new AchievementName
                                                         {
                                                             AchievementId = achievementName.AchievementId,
                                                             Language = achievementName.Language,
                                                             Name = achievementName.Name,
                                                             Description = achievementName.Description
                                                         };
                _repository.InsertOnSubmit(newAchievementName);
            }

            _repository.SubmitChanges();
        }

        /// <summary>
        /// Gets the missing database achievements.
        /// </summary>
        /// <param name="communityAchievements">The community achievements.</param>
        /// <returns></returns>
        public ICollection<Achievement> GetMissingAchievements(ICollection<Achievement> communityAchievements)
        {
            if (communityAchievements == null)
            {
                throw new ArgumentNullException("communityAchievements");
            }

            if (communityAchievements.Count == 0)
            {
                return new Achievement[0];
            }

            var communityGameIds = communityAchievements.Select(a => a.GameId).Distinct();
            var communityApiNames = communityAchievements.Select(a => a.ApiName.Trim()).Distinct();

            var dbAchievements =
                (from achievement in _repository.Achievements
                 where communityGameIds.Contains(achievement.GameId)
                    && communityApiNames.Contains(achievement.ApiName.Trim())
                 select new { achievement.GameId, ApiName = achievement.ApiName.Trim() })
                 .ToList();

            List<Achievement> missingAchievements = new List<Achievement>();
            if (communityAchievements.Count != dbAchievements.Count)
            {
                foreach (Achievement achievement in communityAchievements)
                {
                    Achievement communityAchievement = achievement;
                    bool exists =
                        dbAchievements.Where(
                            a => a.GameId == communityAchievement.GameId
                                 && String.Compare(a.ApiName.Trim(), communityAchievement.ApiName.Trim(), ignoreCase: true) == 0)
                            .Any();

                    if (!exists)
                    {
                        missingAchievements.Add(communityAchievement);
                    }
                }
            }

            return missingAchievements;
        }

        /// <summary>
        /// Gets the missing database achievement names.
        /// </summary>
        /// <param name="communityAchievements">The community achievements.</param>
        /// <returns></returns>
        private ICollection<AchievementName> GetMissingAchievementNames(ICollection<Achievement> communityAchievements)
        {
            if (communityAchievements == null)
            {
                throw new ArgumentNullException("communityAchievements");
            }

            if (communityAchievements.Count == 0)
            {
                return new AchievementName[0];
            }

            var communityAchievementKeys =
                from achievement in communityAchievements
                from name in achievement.AchievementNames
                select new
                           {
                               achievement.GameId,
                               achievement.ApiName,
                               name.Language
                           };

            IEnumerable<int> gameIds = communityAchievementKeys.Select(key => key.GameId).Distinct();
            IEnumerable<string> apiNames = communityAchievementKeys.Select(key => key.ApiName).Distinct();

            var dbAchievementNames =
                (from achievement in _repository.Achievements
                 where gameIds.Contains(achievement.GameId)
                       && apiNames.Contains(achievement.ApiName)
                 select
                     new
                         {
                             achievement.Id,
                             achievement.GameId,
                             achievement.ApiName,
                             Languages = achievement.AchievementNames.Select(name => name.Language)
                         }).ToList();

            List<AchievementName> missingAchievementNames = new List<AchievementName>();
            foreach (AchievementName achievementName in communityAchievements.SelectMany(a => a.AchievementNames))
            {
                AchievementName communityName = achievementName;
                var key =
                    dbAchievementNames.Where(
                        a => a.GameId == communityName.Achievement.GameId
                             && a.ApiName == communityName.Achievement.ApiName)
                        .SingleOrDefault();

                if (key != null && !key.Languages.Contains(communityName.Language))
                {
                    missingAchievementNames.Add(new AchievementName
                                                    {
                                                        AchievementId = key.Id,
                                                        Language = communityName.Language,
                                                        Name = communityName.Name,
                                                        Description = communityName.Description
                                                    });
                }
            }

            return missingAchievementNames;
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
    }
}