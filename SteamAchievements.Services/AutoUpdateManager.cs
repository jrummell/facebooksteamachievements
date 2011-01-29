﻿#region License

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
using System.Text;
using Facebook;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    //TODO: remove dependency on IAchievementManager since it already depends on IAchievementService?
    public class AutoUpdateManager : IDisposable
    {
        private readonly IAchievementManager _achievementManager;
        private readonly IAchievementService _achievementService;
        private readonly IAutoUpdateLogger _log;
        private readonly IFacebookPublisher _publisher;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoUpdateManager"/> class.
        /// </summary>
        /// <param name="log">The log.</param>
        public AutoUpdateManager(IAutoUpdateLogger log)
            : this(null, null, null, log)
        {
        }

        /// <summary>
        /// Constructor for unit testing.
        /// </summary>
        /// <param name="achievementService">The achievement service.</param>
        /// <param name="achievementManager">The achievement manager.</param>
        /// <param name="publisher">The publisher.</param>
        /// <param name="log">The log.</param>
        public AutoUpdateManager(IAchievementService achievementService, IAchievementManager achievementManager,
                                 IFacebookPublisher publisher, IAutoUpdateLogger log)
        {
            _achievementService = achievementService ?? new AchievementService();
            _achievementManager = achievementManager ?? new AchievementManager();
            _publisher = publisher ?? new FacebookPublisher();
            _log = log;
        }

        #region IDisposable Members

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
        /// Gets the auto update steam user ids.
        /// </summary>
        public string GetAutoUpdateUsers()
        {
            // get users configured for auto update
            string[] steamUserIds = _achievementManager.GetAutoUpdateUsers().Select(user => user.SteamUserId).ToArray();

            return String.Join(";", steamUserIds);
        }

        /// <summary>
        /// Publishes the user's achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        public void PublishUserAchievements(string steamUserId)
        {
            User user = _achievementManager.GetUser(steamUserId);

            if (user == null)
            {
                _log.Log(steamUserId + " does not exist");
                return;
            }

            _log.Log("User " + user.SteamUserId + " (" + user.FacebookUserId + ")");

            if (String.IsNullOrEmpty(user.AccessToken))
            {
                _log.Log("Empty AccessToken");

                // if there is no access token, the user hasn't given the app offline_access
                return;
            }

            // update the user's achievements
            int updated = _achievementService.UpdateAchievements(user.SteamUserId);

            if (updated == 0)
            {
                _log.Log("No updated achievements");

                return;
            }

            // get unpublished achievements earned in the last 24-48 hours to make up for time zone differences 
            // and the time it takes to run the Auto Update process
            DateTime oldestDate = DateTime.UtcNow.AddHours(-48).Date;
            IEnumerable<SimpleAchievement> achievements =
                _achievementService.GetUnpublishedAchievements(user.SteamUserId, oldestDate);

            if (!achievements.Any())
            {
                _log.Log("No unpublished achievements");

                return;
            }

            // only publish the top 5
            achievements = achievements.Take(5);

            // since the api only supports one attachment, use the first achievement's image
            // and build the description from all achievements
            SimpleAchievement firstAchievement = achievements.First();
            Uri statsUrl = SteamCommunityManager.GetStatsUrl(user.SteamUserId);
            string message = String.Format("{0} earned new achievements", user.SteamUserId);

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("link", statsUrl.ToString());
            parameters.Add("message", message);
            parameters.Add("name", firstAchievement.Name);
            parameters.Add("picture", firstAchievement.ImageUrl);
            parameters.Add("description", BuildDescription(achievements));

            List<int> publishedAchievements = new List<int>();
            try
            {
                // publish the post
                _publisher.Publish(user, parameters);

                publishedAchievements.AddRange(achievements.Select(a => a.Id));
            }
            catch (FacebookApiException ex)
            {
                _log.Log(ex);

                return;
            }

            // update the published flag
            _achievementService.PublishAchievements(user.SteamUserId, publishedAchievements);

            _log.Log("User achievements published");

            return;
        }

        /// <summary>
        /// Builds the post description
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        /// <returns></returns>
        private static string BuildDescription(IEnumerable<SimpleAchievement> achievements)
        {
            StringBuilder message = new StringBuilder();

            int currentGameId = 0;
            foreach (SimpleAchievement achievement in achievements)
            {
                message.Append(" ");

                if (currentGameId != achievement.Game.Id)
                {
                    message.Append(achievement.Game.Name).Append(":");
                    currentGameId = achievement.Game.Id;
                }

                message.AppendFormat(" {0} ({1}),", achievement.Name, achievement.Description);
            }

            // remove the last comma
            message.Remove(message.Length - 1, 1);

            return message.ToString();
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
                    _achievementManager.Dispose();
                    _achievementService.Dispose();
                }
            }
        }
    }
}