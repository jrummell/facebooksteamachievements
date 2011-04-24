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
using System.Text;
using Facebook;

namespace SteamAchievements.Services
{
    public class AutoUpdateManager : IAutoUpdateManager
    {
        private readonly StringBuilder _achievementManagerLog = new StringBuilder();
        private readonly IAchievementService _achievementService;
        private readonly IAutoUpdateLogger _log;
        private readonly IFacebookPublisher _publisher;
        private readonly IUserService _userService;

        /// <summary>
        /// Constructor for unit testing.
        /// </summary>
        /// <param name="achievementService">The achievement service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="publisher">The publisher.</param>
        /// <param name="log">The log.</param>
        public AutoUpdateManager(IAchievementService achievementService, IUserService userService,
                                 IFacebookPublisher publisher, IAutoUpdateLogger log)
        {
            if (achievementService == null)
            {
                throw new ArgumentNullException("achievementService");
            }

            if (userService == null)
            {
                throw new ArgumentNullException("userService");
            }

            if (publisher == null)
            {
                throw new ArgumentNullException("publisher");
            }

            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _achievementService = achievementService;
            _userService = userService;
            _publisher = publisher;
            _log = log;
        }

        #region IAutoUpdateManager Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// Gets the auto update steam user ids.
        /// </summary>
        public string GetAutoUpdateUsers()
        {
            // get users configured for auto update
            IEnumerable<string> steamUserIds = _userService.GetAutoUpdateUsers();

            return String.Join(";", steamUserIds);
        }

        /// <summary>
        /// Publishes the user's achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        public void PublishUserAchievements(string steamUserId)
        {
            User user = _userService.GetUser(steamUserId);

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
            Uri statsUrl = SteamCommunityManager.GetProfileUrl(user.SteamUserId, false);
            string message = String.Format("{0} earned new achievements", user.SteamUserId);

            IDictionary<string, object> parameters =
                new Dictionary<string, object>
                    {
                        {"link", statsUrl.ToString()},
                        {"message", message},
                        {"name", firstAchievement.Name},
                        {"picture", firstAchievement.ImageUrl},
                        {"description", BuildDescription(achievements, user.PublishDescription)}
                    };

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

        #endregion

        /// <summary>
        /// Builds the post description
        /// </summary>
        /// <param name="achievements">The achievements.</param>
        /// <param name="publishDescription">if set to <c>true</c> [publish description].</param>
        /// <returns></returns>
        private static string BuildDescription(IEnumerable<SimpleAchievement> achievements, bool publishDescription)
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

                message.AppendFormat(" {0}", achievement.Name);

                if (publishDescription)
                {
                    message.AppendFormat(" ({0})", achievement.Description);
                }

                message.Append(",");
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
                    _log.Log("SQL:");
                    _log.Log(_achievementManagerLog.ToString());

                    _userService.Dispose();
                    _achievementService.Dispose();
                }
            }
        }
    }
}