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
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;

namespace SteamAchievements.Data
{
    public class AchievementCollection : Collection<Achievement>
    {
        public AchievementCollection(IList<Achievement> achievements, string steamId, string gameId)
            : base(achievements)
        {
            SteamUserId = steamId;
            GameId = gameId;
        }

        public AchievementCollection(string steamId, string gameId)
        {
            SteamUserId = steamId;
            GameId = gameId;
        }

        public AchievementCollection()
        {
        }

        public string SteamUserId { get; private set; }

        public string GameId { get; private set; }

        public SyndicationFeed ToSyndicationFeed()
        {
            string statsUrl = String.Format("http://steamcommunity.com/id/{0}/stats/{1}?tab=achievements", SteamUserId,
                                            GameId);

            SyndicationFeed feed = new SyndicationFeed(SteamUserId + "'s Achievements",
                                                       "The Steam Achievements feed for " + SteamUserId,
                                                       new Uri(statsUrl))
                                       {
                                           LastUpdatedTime = new DateTimeOffset(DateTime.Now)
                                       };

            List<SyndicationItem> items = new List<SyndicationItem>();

            foreach (Achievement achievement in this)
            {
                SyndicationItem item = new SyndicationItem {Title = new TextSyndicationContent(achievement.Name)};

                string summary = String.Format("<img src=\"{0}\" alt=\"\" /> {1}", achievement.ImageUrl,
                                               achievement.Description);
                item.Summary = new TextSyndicationContent(summary);

                items.Add(item);
            }

            feed.Items = items;

            return feed;
        }
    }
}