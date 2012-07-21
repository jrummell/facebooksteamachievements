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
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services
{
    public class AchievementXmlParser : IAchievementXmlParser
    {
        private readonly TextInfo _textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

        #region IAchievementXmlParser Members

        /// <summary>
        /// Returns a collection of <see cref="UserAchievement"/>s from the given xml and gameId.
        /// </summary>
        public ICollection<UserAchievement> Parse(string xml)
        {
            return Parse(xml, false);
        }

        /// <summary>
        /// Returns a collection of closed <see cref="UserAchievement"/>s from the given xml.
        /// </summary>
        public ICollection<UserAchievement> ParseClosed(string xml)
        {
            return Parse(xml, true);
        }

        #endregion

        /// <summary>
        /// Returns a collection of <see cref="UserAchievement"/>s from the given xml.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="closedOnly">If true, get only closed achievements, else get all achievements.</param>
        /// <returns></returns>
        private ICollection<UserAchievement> Parse(string xml, bool closedOnly)
        {
            XDocument document = XDocument.Parse(xml);

            // xpath: player/customURL
            XElement playerElement = document.Descendants("player").FirstOrDefault();
            if (playerElement == null)
            {
                // game is missing xml achievements
                return new List<UserAchievement>();
            }

            XElement customUrlElement = playerElement.Element("customURL");

            var achievements =
                from element in document.Descendants("achievement")
                let apiname = element.Element("apiname")
                let iconClosed = element.Element("iconClosed")
                let description = element.Element("description")
                let name = element.Element("name")
                let closed = element.Attribute("closed")
                let date = element.Element("unlockTimestamp")
                where apiname != null && iconClosed != null && description != null && name != null && closed != null
                select new
                           {
                               apiname = apiname.Value.Trim(),
                               closed = closed.Value == "1",
                               // name is in all caps - fix it
                               name = _textInfo.ToTitleCase(name.Value.ToLower()),
                               description = description.Value,
                               image = iconClosed.Value,
                               date = date == null ? null : date.Value
                           };

            if (closedOnly)
            {
                achievements = from achievement in achievements
                               where achievement.closed
                               select achievement;
            }

            return (from achievement in achievements
                    select new UserAchievement
                               {
                                   SteamUserId = customUrlElement.Value,
                                   Closed = achievement.closed,
                                   Date = GetDate(achievement.date),
                                   Achievement =
                                       new Achievement
                                           {
                                               ApiName = achievement.apiname,
                                               Name = achievement.name,
                                               Description = achievement.description,
                                               ImageUrl =
                                                   new Uri(achievement.image, UriKind.Absolute).ToString()
                                           }
                               }).ToList();
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> from a unix timestamp.
        /// </summary>
        /// <param name="unixTimestamp">The unix timestamp.</param>
        /// <returns></returns>
        private static DateTime GetDate(string unixTimestamp)
        {
            try
            {
                long value = Convert.ToInt64(unixTimestamp);
                return new DateTime(1970, 1, 1).AddSeconds(value);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
    }
}