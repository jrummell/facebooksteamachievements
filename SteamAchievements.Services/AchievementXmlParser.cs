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

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    public class AchievementXmlParser
    {
        private readonly TextInfo _textInfo = Thread.CurrentThread.CurrentCulture.TextInfo;

        /// <summary>
        /// Returns a collection of <see cref="Achievement"/>s from the given xml and gameId.
        /// </summary>
        /// <param name="gameId">The game id.</param>
        /// <param name="xml">The XML.</param>
        /// <param name="closedOnly">If true, get only closed achievements, else get all achievements.</param>
        /// <returns></returns>
        public IEnumerable<Achievement> Parse(string xml, int gameId, bool closedOnly)
        {
            XDocument document = XDocument.Parse(xml);

            var achievements =
                from element in document.Descendants("achievement")
                let closed = element.Attribute("closed")
                let name = element.Element("name")
                let description = element.Element("description")
                let image = element.Element("iconClosed")
                where closed != null && name != null
                    && description != null && image != null
                select new
                        {
                            closed = closed.Value == "1",
                            // name is in all caps - fix it
                            name = _textInfo.ToTitleCase(name.Value.ToLower()),
                            description = description.Value,
                            image = image.Value
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
                                  GameId = gameId,
                                  Name = achievement.name,
                                  Description = achievement.description,
                                  ImageUrl = achievement.image
                              };
        }
    }
}