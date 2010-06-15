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

namespace SteamAchievements.Services
{
    public class AchievementXmlParser : IXmlParser<Achievement>
    {
        private readonly TextInfo _textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

        #region IXmlParser<Achievement> Members

        /// <summary>
        /// Returns a collection of <see cref="Achievement"/>s from the given xml and gameId.
        /// </summary>
        public IEnumerable<Achievement> Parse(string xml)
        {
            return Parse(xml, false);
        }

        #endregion

        /// <summary>
        /// Returns a collection of closed <see cref="Achievement"/>s from the given xml.
        /// </summary>
        public IEnumerable<Achievement> ParseClosed(string xml)
        {
            return Parse(xml, true);
        }

        /// <summary>
        /// Returns a collection of <see cref="Achievement"/>s from the given xml.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="closedOnly">If true, get only closed achievements, else get all achievements.</param>
        /// <returns></returns>
        private IEnumerable<Achievement> Parse(string xml, bool closedOnly)
        {
            XDocument document = XDocument.Parse(xml);

            var achievements =
                from element in document.Descendants("achievement")
                select new
                           {
                               closed = element.Attribute("closed").Value == "1",
                               // name is in all caps - fix it
                               name = _textInfo.ToTitleCase(element.Element("name").Value.ToLower()),
                               description = element.Element("description").Value,
                               image = element.Element("iconClosed").Value
                           };

            if (closedOnly)
            {
                achievements = from achievement in achievements
                               where achievement.closed
                               select achievement;
            }


            return from achievement in achievements
                   select new Achievement
                              {
                                  Name = achievement.name,
                                  Description = achievement.description,
                                  ImageUrl = new Uri(achievement.image, UriKind.Absolute),
                                  Closed = achievement.closed
                              };
        }
    }
}