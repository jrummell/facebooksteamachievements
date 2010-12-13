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
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using Elmah;
using Facebook;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements.Admin
{
    public partial class AutoUpdate : Page
    {
        private static readonly StringBuilder _log = new StringBuilder();
        private static string _fullLogPath;
        private static readonly IAchievementService _achievementService = new AchievementService();
        private static readonly object _publishLock = new object();
        private static readonly object _logLock = new object();

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += Page_Load;
            Unload += Page_Unload;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            InitLog();

            Log("Auto Update start");

            bool authorized = Request["auth"] == Properties.Settings.Default.AutoUpdateAuthKey;
            if (!authorized)
            {
                Log("Invalid auth key");
                unauthorizedDiv.Visible = true;
            }
            else
            {
                ThreadPool.QueueUserWorkItem(PublishAchievements);
            }

            FlushLog();
        }

        /// <summary>
        /// Handles the Unload event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void Page_Unload(object sender, EventArgs e)
        {
            _achievementService.Dispose();
        }

        /// <summary>
        /// Publishes the achievements.
        /// </summary>
        /// <returns></returns>
        private static void PublishAchievements(object state)
        {
            try
            {
                lock (_publishLock)
                {
                    IEnumerable<User> users;
                    using (IAchievementManager manager = new AchievementManager())
                    {
                        // get users configured for auto update
                        users = manager.GetAutoUpdateUsers();
                    }

                    Log("Auto Update user count: " + users.Count());
                    FlushLog();

                    int logCount = 0;

                    foreach (User user in users)
                    {
                        PublishUserAcheivements(user);

                        // flush the log every 10 users - log often to increase chances of catching errors.
                        if (logCount%10 == 0)
                        {
                            FlushLog();
                        }

                        logCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                LogException(ex);

                // since this is executed in a separate thread, Elmah won't log this without some help.
                ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
            }
            finally
            {
                Log("Auto Update done");
                FlushLog();
            }
        }

        /// <summary>
        /// Publishes the user's acheivements.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private static void PublishUserAcheivements(User user)
        {
            Log("User: " + user.SteamUserId + " (" + user.FacebookUserId + ")");

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

            // get the user's unpublished achievements
            IEnumerable<SimpleAchievement> achievements = _achievementService.GetNewAchievements(user.SteamUserId);

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
        private static void InitLog()
        {
            string logPath = HttpContext.Current.Server.MapPath("~/App_Data/AutoUpdate");
            string logFileName = DateTime.UtcNow.Ticks + ".log";
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
            Log("Exception: " + ex.Message);

            if (ex.InnerException != null)
            {
                Log("Inner Exception: " + ex.InnerException.Message);
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
    }
}