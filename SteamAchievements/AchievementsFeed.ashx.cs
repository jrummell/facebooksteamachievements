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
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements
{
    public class AchievementsFeed : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            string steamId = context.Request.QueryString["user"];
            string gameIdString = context.Request.QueryString["game"];
            string live = context.Request.QueryString["live"];

            if (String.IsNullOrEmpty(steamId) || String.IsNullOrEmpty(gameIdString))
            {
                context.Response.Write("missing user or game");
            }

            int gameId;
            try
            {
                gameId = Convert.ToInt32(gameIdString);
            }
            catch (FormatException)
            {
                context.Response.Write("invalid game");
                return;
            }

            context.Response.ContentType = "application/rss+xml";

            XmlWriter writer = new XmlTextWriter(context.Response.Output);

            AchievementManager service = new AchievementManager();
            AchievementCollection achievements = service.GetAchievements(steamId, gameId);

            if (!String.IsNullOrEmpty(live))
            {
                achievements = new SteamCommunityManager().GetAchievements(steamId);
            }

            SyndicationFeed feed = achievements.ToSyndicationFeed();

            feed.SaveAsRss20(writer);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}