using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Facebook;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    //TODO: write unit tests, remove dependency on IAchievementManager since it already depends on IAchievementService.
    public class AutoUpdateManager : IDisposable
    {
        private readonly IAchievementService _achievementService;
        private readonly IAchievementManager _achievementManager;
        private readonly AutoUpdateLogger _log;

        public AutoUpdateManager(AutoUpdateLogger log)
            : this(null, null, log)
        {
        }

        public AutoUpdateManager(IAchievementService achievementService, IAchievementManager achievementManager, AutoUpdateLogger log)
        {
            _achievementService = achievementService ?? new AchievementService();
            _achievementManager = achievementManager ?? new AchievementManager();
            _log = log;
        }

        /// <summary>
        /// Gets the auto update steam user ids.
        /// </summary>
        /// <returns></returns>
        public string GetAutoUpdateUsers()
        {
            // get users configured for auto update
            string[] steamUserIds = _achievementManager.GetAutoUpdateUsers().Select(user => user.SteamUserId).ToArray();

            return String.Join(";", steamUserIds);
        }

        /// <summary>
        /// Publishes the user's achievements.
        /// </summary>
        /// <param name="steamUserId"></param>
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

            // get unpublished achievements earned in the last 48 hours to make up for time zone differences 
            // and the time it takes to run the Auto Update process
            DateTime oldestDate = DateTime.UtcNow.AddHours(-48);
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

            dynamic parameters = new ExpandoObject();
            parameters.link = statsUrl.ToString();
            parameters.message = message;
            parameters.name = firstAchievement.Name;
            parameters.picture = firstAchievement.ImageUrl;
            parameters.description = BuildDescription(achievements);

            List<int> publishedAchievements = new List<int>();
            try
            {
                // publish the post
                FacebookApp app = new FacebookApp(user.AccessToken);
                string userFeedPath = String.Format("/{0}/feed/", user.FacebookUserId);
                app.Api(userFeedPath, parameters, HttpMethod.Post);

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
        /// <param name="achievements"></param>
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

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }
    }
}
