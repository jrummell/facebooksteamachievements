using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Facebook;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements.Admin
{
    public class AutoUpdate : IHttpHandler
    {
        private static readonly StringBuilder _log = new StringBuilder();
        private static string _fullLogPath;
        private readonly IAchievementService _achievementService = new AchievementService();
        private static readonly object _logLock = new object();

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                InitLog(context);

                bool authorized = context.Request["auth"] == Properties.Settings.Default.AutoUpdateAuthKey;
                if (!authorized)
                {
                    Log("Invalid auth key");
                    context.Response.Write("Invalid auth key");
                }
                else
                {
                    string method = context.Request["method"];

                    if (method == "GetAutoUpdateUsers")
                    {
                        Log("Getting auto update users");

                        string users = GetAutoUpdateUsers();

                        Log(users);

                        FlushLog();

                        context.Response.Write(users);
                    }
                    else if (method == "PublishUserAchievements")
                    {
                        string userName = context.Request["user"];

                        PublishUserAchievements(userName);

                        FlushLog();

                        context.Response.Write(userName + " published.");
                    }
                    else
                    {
                        context.Response.Write("Invalid method");
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
                WriteLog(context.Response);
            }
            finally
            {
                FlushLog();
                _achievementService.Dispose();
            }
        }

        /// <summary>
        /// Gets the auto update steam user ids.
        /// </summary>
        /// <returns></returns>
        private string GetAutoUpdateUsers()
        {
            string[] steamUserIds;
            using (IAchievementManager manager = new AchievementManager())
            {
                // get users configured for auto update
                steamUserIds = manager.GetAutoUpdateUsers().Select(user => user.SteamUserId).ToArray();
            }

            return String.Join(";", steamUserIds);
        }

        /// <summary>
        /// Publishes the user's achievements.
        /// </summary>
        /// <param name="steamUserId"></param>
        private void PublishUserAchievements(string steamUserId)
        {
            User user;
            using (IAchievementManager manager = new AchievementManager())
            {
                // get users configured for auto update
                user = manager.GetUser(steamUserId);
            }

            if (user == null)
            {
                Log(steamUserId + " does not exist");
                return;
            }

            Log("User " + user.SteamUserId + " (" + user.FacebookUserId + ")");

            if (String.IsNullOrEmpty(user.AccessToken))
            {
                Log("Empty AccessToken");

                // if there is no access token, the user hasn't given the app offline_access
                return;
            }

            // update the user's achievements
            int updated = _achievementService.UpdateAchievements(user.SteamUserId);

            if (updated == 0)
            {
                Log("No updated achievements");

                return;
            }

            // get unpublished achievements earned in the last 48 hours to make up for time zone differences 
            // and the time it takes to run the Auto Update process
            DateTime oldestDate = DateTime.UtcNow.AddHours(-48);
            IEnumerable<SimpleAchievement> achievements = 
                _achievementService.GetUnpublishedAchievements(user.SteamUserId, oldestDate);

            if (!achievements.Any())
            {
                Log("No unpublished achievements");

                return;
            }

            FacebookApp app = new FacebookApp(user.AccessToken);
            string userFeedPath = String.Format("/{0}/feed/", user.FacebookUserId);

            List<int> publishedAchievements = new List<int>();

            // post the first 5 new achievements
            foreach (SimpleAchievement achievement in achievements.Take(5))
            {
                string message = String.Format("{0} earned an achievement in {1}",
                                               user.SteamUserId, achievement.Game.Name);
                dynamic parameters = new ExpandoObject();
                parameters.link = achievement.Game.StatsUrl;
                parameters.message = message;
                parameters.name = achievement.Name;
                parameters.description = achievement.Description;
                parameters.picture = achievement.ImageUrl;

                Log(message + ": " + achievement.Name);

                try
                {
                    app.Api(userFeedPath, parameters, HttpMethod.Post);

                    publishedAchievements.Add(achievement.Id);
                }
                catch (FacebookApiException ex)
                {
                    LogException(ex);
                    FlushLog();
                }
            }

            // update the published flag
            _achievementService.PublishAchievements(user.SteamUserId, publishedAchievements);

            Log("User achievements published");

            return;
        }

        /// <summary>
        /// Inits the log.
        /// </summary>
        private static void InitLog(HttpContext context)
        {
            string logPath = context.Server.MapPath("~/App_Data/AutoUpdate");
            DateTime now = DateTime.UtcNow;
            string logFileName = String.Format("{0}-{1}-{2}.log", now.Year, now.Month, now.Day);
            _fullLogPath = Path.Combine(logPath, logFileName);
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void Log(string message)
        {
            _log.AppendFormat("{0} {1}{2}", DateTime.UtcNow, message, Environment.NewLine);
        }

        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        private static void LogException(Exception ex)
        {
            Log("Exception: " + ex.GetType());
            Log(ex.Message);
            Log(ex.StackTrace);

            if (ex.InnerException != null)
            {
                Log("Inner Exception: " + ex.InnerException.GetType());
                Log(ex.InnerException.Message);
                Log(ex.InnerException.StackTrace);
            }
        }

        /// <summary>
        /// Flushes the log.
        /// </summary>
        private static void FlushLog()
        {
            lock (_logLock)
            {
                File.AppendAllText(_fullLogPath, _log.ToString());
                _log.Clear();
            }
        }

        /// <summary>
        /// Writes the log to the response.
        /// </summary>
        /// <param name="response"></param>
        private static void WriteLog(HttpResponse response)
        {
            response.Write(_log.ToString());
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}