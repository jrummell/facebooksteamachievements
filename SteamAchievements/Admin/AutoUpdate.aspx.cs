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
using System.Web.UI;
using Facebook;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements.Admin
{
    public partial class AutoUpdate : Page
    {
        private readonly StringBuilder _log = new StringBuilder();
        private string _fullLogPath;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += Page_Load;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
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
                    FlushLog();

                    List<Result> results = UpdateAchievements();

                    FlushLog();

                    userRepeater.DataSource = results;
                    userRepeater.DataBind();

                    if (results.Count == 0)
                    {
                        Log("No users had new unpublished achievements");

                        noUpdatesDiv.Visible = true;
                    }
                }

                Log("Done");
            }
            catch (Exception ex)
            {
                Log("Exception: " + ex.Message);

                if (ex.InnerException != null)
                {
                    Log("Inner Exception: " + ex.InnerException.Message);
                }
            }

            FlushLog();
        }

        private List<Result> UpdateAchievements()
        {
            IEnumerable<User> users;
            using (IAchievementManager manager = new AchievementManager())
            {
                // get users configured for auto update
                users = manager.GetAutoUpdateUsers();
            }

            Log("Auto Update user count: " + users.Count());

            List<Result> results = new List<Result>();

            using (IAchievementService service = new AchievementService())
            {
                foreach (User user in users)
                {
                    Log("User: " + user.SteamUserId + " (" + user.FacebookUserId + ")");

                    if (String.IsNullOrEmpty(user.AccessToken))
                    {
                        Log("Empty AccessToken");

                        // if there is no access token, the user hasn't given the app offline_access
                        continue;
                    }

                    // update the user's achievements
                    int updated = service.UpdateAchievements(user.SteamUserId);

                    if (updated == 0)
                    {
                        Log("No updated achievements");

                        continue;
                    }

                    // get the user's unpublished achievements
                    IEnumerable<SimpleAchievement> achievements = service.GetNewAchievements(user.SteamUserId);

                    if (!achievements.Any())
                    {
                        Log("No unpublished achievements");

                        continue;
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

                        Log(message);

                        Result result = new Result
                                            {
                                                SteamUserId = user.SteamUserId,
                                                GameName = achievement.Game.Name,
                                                Description = achievement.Name
                                            };

                        try
                        {
                            app.Api(userFeedPath, parameters, HttpMethod.Post);

                            publishedAchievements.Add(achievement.Id);
                        }
                        catch (FacebookApiException ex)
                        {
                            // log Facebook errors and continue
                            result.ExceptionMessage += Environment.NewLine + "Exception: " + ex.Message;
                            if (ex.InnerException != null)
                            {
                                result.ExceptionMessage += Environment.NewLine + ", Inner Exception: " +
                                                           ex.InnerException.Message;
                            }

                            Log(result.ExceptionMessage);
                        }

                        results.Add(result);
                    }

                    // update the published flag
                    service.PublishAchievements(user.SteamUserId, publishedAchievements);

                    Log("User achievements published");
                }
            }

            return results;
        }

        private void InitLog()
        {
            string logPath = Server.MapPath("~/App_Data/AutoUpdate");
            string logFileName = DateTime.UtcNow.Ticks + ".log";
            _fullLogPath = Path.Combine(logPath, logFileName);
        }

        private void Log(string message)
        {
            _log.AppendFormat("{0} {1}{2}", DateTime.UtcNow, message, Environment.NewLine);
        }

        private void FlushLog()
        {
            File.AppendAllText(_fullLogPath, _log.ToString());
            _log.Clear();
        }

        #region Nested type: Result

        private class Result
        {
            public string SteamUserId { get; set; }
            public string GameName { get; set; }
            public string Description { get; set; }
            public string ExceptionMessage { get; set; }
        }

        #endregion
    }
}