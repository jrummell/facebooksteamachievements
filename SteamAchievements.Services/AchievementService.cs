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
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    [ServiceErrorBehaviour(typeof (HttpErrorHandler))]
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementManager _achievementManager;
        private readonly SteamCommunityManager _communityService;

        public AchievementService()
        {
            _achievementManager = new AchievementManager();
            _communityService = new SteamCommunityManager();
        }

        #region IAchievementService Members

        /// <summary>
        /// Gets the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="gameId">The game id.</param>
        /// <returns>
        /// All <see cref="Achievement"/>s for the given user and game.
        /// </returns>
        public List<SimpleAchievement> GetAchievements(string steamUserId, int gameId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            return _achievementManager.GetAchievements(steamUserId, gameId).ToSimpleAchievementList();
        }

        /// <summary>
        /// Gets the games.
        /// </summary>
        /// <returns>All <see cref="Game"/>s.</returns>
        public List<SimpleGame> GetGames(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            return _communityService.GetGames(steamUserId).ToSimpleGameList();
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        public int UpdateAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            IEnumerable<Achievement> achievements = _communityService.GetAchievements(steamUserId);

            int updated = _achievementManager.UpdateAchievements(steamUserId, achievements.ToDataAchievements());

            return updated;
        }

        /// <summary>
        /// Updates the steam user id.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>true if successful, else false.</returns>
        /// <remarks>jQuery/WCF requires a return value in order for jQuery to execute $.ajax.success.</remarks>
        public bool UpdateSteamUserId(long facebookUserId, string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            _achievementManager.UpdateSteamUserId(facebookUserId, steamUserId);

            return true;
        }

        /// <summary>
        /// Publishes the last 5 achievements added within the last 5 minutes for the given user.
        /// </summary>
        /// <param name="facebookUserId">The facebook user id.</param>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>true if successful, else false.</returns>
        public bool PublishLatestAchievements(long facebookUserId, string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            IEnumerable<Data.Achievement> dataAchievements = _achievementManager.GetLatestAchievements(steamUserId);
            IEnumerable<Game> games = from game in _communityService.GetGames(steamUserId)
                                      where game.PlayedRecently
                                      select game;

            if (dataAchievements.Any())
            {
                AchievementsPublisher publisher = new AchievementsPublisher();
                publisher.Publish(dataAchievements.ToAchievements(games), steamUserId, facebookUserId);

                _achievementManager.UpdatePublished(steamUserId, dataAchievements.Select(achievement => achievement.Id));
            }

            return true;
        }

        public void Dispose()
        {
            _achievementManager.Dispose();
            _communityService.Dispose();
        }

        public List<SimpleAchievement> GetNewAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            return _achievementManager.GetUnpublishedAchievements(steamUserId).ToSimpleAchievementList();
        }

        public bool PublishAchievements(string steamUserId, IEnumerable<int> achievementIds)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            if (achievementIds == null)
            {
                throw new ArgumentNullException("achievementIds");
            }

            _achievementManager.UpdatePublished(steamUserId, achievementIds);

            return true;
        }

        #endregion
    }
}