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
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    [ServiceErrorBehaviour(typeof (HttpErrorHandler))]
    public class AchievementService : IAchievementService
    {
        private readonly IAchievementManager _achievementManager;
        private readonly SteamCommunityManager _communityService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AchievementService"/> class.
        /// </summary>
        public AchievementService()
        {
            _achievementManager = new AchievementManager();
            _communityService = new SteamCommunityManager();
        }

        #region IAchievementService Members

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

            IEnumerable<UserAchievement> achievements = _communityService.GetAchievements(steamUserId);

            User user = _achievementManager.GetUser(steamUserId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.", "steamUserId");
            }

            int updated = _achievementManager.UpdateAchievements(achievements.ToDataAchievements(user.FacebookUserId));

            return updated;
        }

        /// <summary>
        /// Gets the new achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns>
        /// The new achievements that haven't been stored yet.
        /// </returns>
        public List<SimpleAchievement> GetNewAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            IEnumerable<Game> games = _communityService.GetGames(steamUserId);

            return _achievementManager.GetUnpublishedAchievements(steamUserId).ToSimpleAchievementList(games);
        }

        /// <summary>
        /// Gets the unpublished achievements newer than the given date.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="oldestDate">The oldest date.</param>
        /// <returns>
        /// The new achievements that haven't been stored yet.
        /// </returns>
        public List<SimpleAchievement> GetUnpublishedAchievements(string steamUserId, DateTime oldestDate)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            IEnumerable<Game> games = _communityService.GetGames(steamUserId);

            return _achievementManager.GetUnpublishedAchievements(steamUserId, oldestDate).ToSimpleAchievementList(games);
        }

        /// <summary>
        /// Updates the Published field for the given achievements for the given user.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <param name="achievementIds">The ids of the achievements to publish.</param>
        /// <returns>
        /// true if successful, else false.
        /// </returns>
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
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing)
                {
                    _achievementManager.Dispose();
                    _communityService.Dispose();
                }
            }
        }
    }
}