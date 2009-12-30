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
using System.Web.Configuration;
using System.Web.Services;
using System.Web.UI;
using Facebook.Rest;
using Facebook.Schema;
using Facebook.Session;
using SteamAchievements.Data;

namespace SteamAchievements
{
    public partial class Default : Page
    {
        /// <summary>
        /// Gets or sets the steam user id.
        /// </summary>
        /// <value>The steam user id.</value>
        protected string SteamUserId { get; private set; }

        /// <summary>
        /// Gets or sets the facebook user id.
        /// </summary>
        /// <value>The facebook user id.</value>
        protected long FacebookUserId { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += Page_Load;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                FacebookUserId = Master.Api.Session.UserId;

                AchievementManager manager = new AchievementManager();
                SteamUserId = manager.GetSteamUserId(FacebookUserId);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        /// <summary>
        /// Posts the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <remarks>This method depends on Master.Api, otherwise it would be in <see cref="SteamAchievements.Services.AchievementService"/>.</remarks>
        [WebMethod]
        public static void PostAchievements(string steamUserId)
        {
            if (steamUserId == null)
            {
                throw new ArgumentNullException("steamUserId");
            }

            string appKey = WebConfigurationManager.AppSettings["APIKey"];
            string appSecret = WebConfigurationManager.AppSettings["Secret"];
            List<Enums.ExtendedPermissions> permissions =
                new List<Enums.ExtendedPermissions> {Enums.ExtendedPermissions.publish_stream};
            IFrameCanvasSession session = new IFrameCanvasSession(appKey, appSecret, permissions);
            Api api = new Api(session);

            AchievementManager manager = new AchievementManager();
            IEnumerable<Achievement> latestAchievements = manager.GetLatestAchievements(steamUserId);

            foreach (Achievement achievement in latestAchievements)
            {
                string description = String.Format("earned the {0} achievement in {1}.", achievement.Name,
                                                   achievement.Game.Name);
                attachment attachment = new attachment
                                            {
                                                caption = achievement.Description,
                                                name = achievement.Name,
                                                href = String.Format("http://steamcommunity.com/id/{0}/stats/{1}",
                                                                     steamUserId, achievement.Game.Abbreviation),
                                                media = new List<attachment_media>
                                                            {
                                                                new attachment_media_image
                                                                    {
                                                                        src = achievement.ImageUrl,
                                                                        href =
                                                                            String.Format(
                                                                            "http://steamcommunity.com/id/{0}/stats/{1}?tab=achievements",
                                                                            steamUserId, achievement.Game.Abbreviation)
                                                                    }
                                                            }
                                            };

                api.Stream.Publish(description, attachment, null, null, 0);
            }
        }

        #region Nested type: PostAchievementsParameter

        public class PostAchievementsParameter
        {
            public string SteamUserId { get; set; }
        }

        #endregion
    }
}