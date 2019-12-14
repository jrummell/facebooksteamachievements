#region License

//  Copyright 2012 John Rummell
//  
//  This file is part of SteamAchievements.
//  
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//  
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;
using UserAchievement = SteamAchievements.Data.UserAchievement;

namespace SteamAchievements.Services
{
    public class AchievementService : Disposable, IAchievementService
    {
        private readonly IAchievementManager _achievementManager;
        private readonly ISteamCommunityManager _communityService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementService" /> class.
        /// </summary>
        /// <param name="achievementManager">The achievement manager.</param>
        /// <param name="communityManager">The community manager.</param>
        /// <param name="mapper">The mapper.</param>
        public AchievementService(IAchievementManager achievementManager, ISteamCommunityManager communityManager, IMapper mapper)
        {
            _achievementManager = achievementManager;
            _communityService = communityManager;
            _mapper = mapper;
        }

        #region IAchievementService Members

        /// <summary>
        ///   Gets the games.
        /// </summary>
        /// <param name="steamUserId"> The steam user id. </param>
        /// <returns> <see cref="GameModel" /> s for the givem steam user id. </returns>
        public ICollection<GameModel> GetGames(string steamUserId)
        {
            return _communityService.GetGames(steamUserId, CultureHelper.GetLanguage()).ToList();
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// true if successful, else false.
        /// </returns>
        public int UpdateAchievements(string userId, string language = null)
        {
            if (language == null)
            {
                language = CultureHelper.GetLanguage();
            }

            string steamUserId = GetSteamUserId(userId);

            var userAchievements = _communityService.GetClosedAchievements(steamUserId, language);

            foreach (var achievement in userAchievements)
            {
                achievement.UserId = userId;
            }

            var dataUserAchievements = _mapper.Map<ICollection<UserAchievement>>(userAchievements);
            int updated = _achievementManager.UpdateAchievements(dataUserAchievements);

            return updated;
        }

        /// <summary>
        /// Gets the unpublished achievements newer than the given date.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The achievements that haven't been published yet.
        /// </returns>
        public ICollection<AchievementModel> GetUnpublishedAchievements(string userId, DateTime? oldestDate,
                                                                   string language = null)
        {
            if (language == null)
            {
                language = CultureHelper.GetLanguage();
            }

            string steamUserId = GetSteamUserId(userId);

            IEnumerable<GameModel> games = _communityService.GetGames(steamUserId, language);

            ICollection<Data.Achievement> dataAchievements;
            if (oldestDate == null)
            {
                dataAchievements = _achievementManager.GetUnpublishedAchievements(userId);
            }
            else
            {
                dataAchievements = _achievementManager.GetUnpublishedAchievements(userId, oldestDate.Value);
            }

            IEnumerable<Data.Achievement> missingNames =
                dataAchievements.Where(a => !a.AchievementNames.Where(n => n.Language == language).Any())
                .ToArray();

            if (missingNames.Any())
            {
                IEnumerable<AchievementModel> communityAchievements =
                    _communityService.GetAchievements(steamUserId, language)
                        .Select(ua => ua.Achievement)
                        .ToArray();

                foreach (Data.Achievement achievement in missingNames)
                {
                    AchievementModel missing =
                        communityAchievements
                            .Where(a => a.Game.Id == achievement.GameId && a.ApiName == achievement.ApiName)
                            .SingleOrDefault();

                    if (missing != null)
                    {
                        achievement.AchievementNames.Add(new AchievementName
                            {
                                Language = language,
                                Name = missing.Name,
                                Description = missing.Description
                            });
                    }
                }
            }

            ICollection<AchievementModel> achievements = _mapper.Map<ICollection<AchievementModel>>(dataAchievements);
            // set game
            foreach (var dataAchievement in dataAchievements)
            {
                var achievement = achievements.Where(a => a.Id == dataAchievement.Id).SingleOrDefault();
                if (achievement != null)
                {
                    achievement.Game = games.Where(g => g.Id == dataAchievement.GameId).SingleOrDefault();
                }
            }

            return achievements.Where(a => a.Game != null)
                .OrderBy(a => a.Game.Name).ThenBy(a => a.Name).ToList();
        }

        /// <summary>
        ///   Updates the new user's achievements and hides any that are more than 2 days old.
        /// </summary>
        /// <param name="user"> The user. </param>
        public void UpdateNewUserAchievements(UserModel user)
        {
            bool exists = _achievementManager.GetUser(user.Id) != null;

            if (!exists)
            {
                throw new ArgumentException("The given user does not exist", nameof(user));
            }

            int updatedCount = UpdateAchievements(user.Id);
            if (updatedCount > 0)
            {
                // hide achievements more than two days old
                ICollection<AchievementModel> achievements =
                    GetUnpublishedAchievements(user.Id, DateTime.UtcNow.Date.AddDays(-2));
                IEnumerable<int> achievementIds = achievements.Select(achievement => achievement.Id);
                HideAchievements(user.Id, achievementIds);
            }
        }

        /// <summary>
        ///   Gets the profile.
        /// </summary>
        /// <param name="steamUserId"> The steam user id. </param>
        /// <returns> </returns>
        public SteamProfileModel GetProfile(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException(nameof(steamUserId));
            }

            return _communityService.GetProfile(steamUserId);
        }

        /// <summary>
        /// Updates the Published field for the given achievements for the given user.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="achievementIds">The ids of the achievements to publish.</param>
        /// <returns>
        /// true if successful, else false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public bool PublishAchievements(string userId, IEnumerable<int> achievementIds)
        {
            if (achievementIds == null)
            {
                throw new ArgumentNullException(nameof(achievementIds));
            }

            _achievementManager.UpdatePublished(userId, achievementIds);

            return true;
        }

        /// <summary>
        /// Hides the given user's achievements.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="achievementIds">The ids of the achievements to hide.</param>
        /// <returns></returns>
        public bool HideAchievements(string userId, IEnumerable<int> achievementIds)
        {
            _achievementManager.UpdateHidden(userId, achievementIds);

            return true;
        }

        #endregion

        private string GetSteamUserId(string userId)
        {
            Data.User user = _achievementManager.GetUser(userId);
            if (user != null)
            {
                return user.SteamUserId;
            }
            throw new ArgumentException($"User {userId} does not exist.", nameof(userId));
        }

        protected override void DisposeManaged()
        {
            _achievementManager.Dispose();
            _communityService.Dispose();
        }
    }
}