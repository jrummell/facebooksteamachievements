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
using AutoMapper;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public class AchievementService : Disposable, IAchievementService
    {
        private readonly IAchievementManager _achievementManager;
        private readonly ISteamCommunityManager _communityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementService"/> class.
        /// </summary>
        /// <param name="achievementManager">The achievement manager.</param>
        /// <param name="communityManager">The community manager.</param>
        public AchievementService(IAchievementManager achievementManager, ISteamCommunityManager communityManager)
        {
            _achievementManager = achievementManager;
            _communityService = communityManager;
        }

        #region IAchievementService Members

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <returns>
        /// All <see cref="Game"/>s.
        /// </returns>
        public ICollection<Game> GetGames(long facebookUserId)
        {
            string steamUserId = GetSteamUserId(facebookUserId);

            return _communityService.GetGames(steamUserId, CultureHelper.GetLanguage()).ToList();
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>
        ///   <see cref="Game"/>s for the givem steam user id.
        /// </returns>
        public ICollection<Game> GetGames(string steamUserId)
        {
            return _communityService.GetGames(steamUserId, CultureHelper.GetLanguage()).ToList();
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// true if successful, else false.
        /// </returns>
        public int UpdateAchievements(long facebookUserId, string language = null)
        {
            if (language == null)
            {
                language = CultureHelper.GetLanguage();
            }

            string steamUserId = GetSteamUserId(facebookUserId);

            var userAchievements = _communityService.GetClosedAchievements(steamUserId, language);
            
            foreach (var achievement in userAchievements) 
            {
            	achievement.FacebookUserId = facebookUserId;
            }

            var dataUserAchievements = Mapper.Map<ICollection<Data.UserAchievement>>(userAchievements);
            int updated = _achievementManager.UpdateAchievements(dataUserAchievements);

            return updated;
        }

        /// <summary>
        /// Gets the unpublished achievements newer than the given date.
        /// </summary>
        /// <param name="facebookUserId"> </param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <param name="language">The language.</param>
        /// <returns>
        /// The achievements that haven't been published yet.
        /// </returns>
        public ICollection<Models.Achievement> GetUnpublishedAchievements(long facebookUserId, DateTime? oldestDate,
                                                                   string language = null)
        {
            if (language == null)
            {
                language = CultureHelper.GetLanguage();
            }

            string steamUserId = GetSteamUserId(facebookUserId);

            IEnumerable<Game> games = _communityService.GetGames(steamUserId, language);

            ICollection<Data.Achievement> dataAchievements;
            if (oldestDate == null)
            {
                dataAchievements = _achievementManager.GetUnpublishedAchievements(facebookUserId);
            }
            else
            {
                dataAchievements = _achievementManager.GetUnpublishedAchievements(facebookUserId, oldestDate.Value);
            }

            IEnumerable<Data.Achievement> missingNames =
                dataAchievements.Where(a => !a.AchievementNames.Where(n => n.Language == language).Any());

            if (missingNames.Any())
            {
                IEnumerable<Models.Achievement> communityAchievements =
                    _communityService.GetAchievements(steamUserId, language)
                        .Select(ua => ua.Achievement);

                foreach (Data.Achievement achievement in missingNames)
                {
                    Models.Achievement missing =
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

            ICollection<Models.Achievement> achievements = Mapper.Map<ICollection<Models.Achievement>>(dataAchievements);
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
        /// Updates the new user's achievements and hides any that are more than 2 days old.
        /// </summary>
        /// <param name="user">The user.</param>
        public void UpdateNewUserAchievements(Models.User user)
        {
            bool exists = _achievementManager.GetUser(user.FacebookUserId) != null;

            if (!exists)
            {
                throw new ArgumentException("The given user does not exist", "user");
            }

            int updatedCount = UpdateAchievements(user.FacebookUserId);
            if (updatedCount > 0)
            {
                // hide achievements more than two days old
                ICollection<Models.Achievement> achievements =
                    GetUnpublishedAchievements(user.FacebookUserId, DateTime.UtcNow.Date.AddDays(-2));
                IEnumerable<int> achievementIds = achievements.Select(achievement => achievement.Id);
                HideAchievements(user.FacebookUserId, achievementIds);
            }
        }

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        public SteamProfile GetProfile(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }
            
            return _communityService.GetProfile(steamUserId);
        }

        /// <summary>
        /// Updates the Published field for the given achievements for the given user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="achievementIds">The ids of the achievements to publish.</param>
        /// <returns>
        /// true if successful, else false.
        /// </returns>
        public bool PublishAchievements(long facebookUserId, IEnumerable<int> achievementIds)
        {
            if (achievementIds == null)
            {
                throw new ArgumentNullException("achievementIds");
            }

            _achievementManager.UpdatePublished(facebookUserId, achievementIds);

            return true;
        }

        /// <summary>
        /// Hides the given user's achievements.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="achievementIds">The ids of the achievements to hide.</param>
        /// <returns></returns>
        public bool HideAchievements(long facebookUserId, IEnumerable<int> achievementIds)
        {
            _achievementManager.UpdateHidden(facebookUserId, achievementIds);

            return true;
        }

        #endregion

        private string GetSteamUserId(long facebookUserId)
        {
            Data.User user = _achievementManager.GetUser(facebookUserId);
            if (user != null)
            {
                return user.SteamUserId;
            }
            throw new ArgumentException("User does not exist.", "facebookUserId");
        }

        protected override void DisposeManaged()
        {
            _achievementManager.Dispose();
            _communityService.Dispose();
        }
    }
}