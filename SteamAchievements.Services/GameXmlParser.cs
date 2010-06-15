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
using System.Linq;
using System.Xml.Linq;

namespace SteamAchievements.Services
{
    public class GameXmlParser : IXmlParser<Game>
    {
        #region IXmlParser<Game> Members

        /// <summary>
        /// Returns a collection of <see cref="Game"/>s from the given xml.
        /// </summary>
        public IEnumerable<Game> Parse(string xml)
        {
            XDocument document = XDocument.Parse(xml);

            var games =
                from element in document.Descendants("game")
                where element.Element("statsLink") != null
                select new
                           {
                               id = element.Element("appID").Value,
                               name = element.Element("name").Value,
                               logo = element.Element("logo").Value,
                               statsLink = element.Element("statsLink").Value,
                               storeLink = element.Element("storeLink").Value
                           };

            return from game in games
                   select new Game
                              {
                                  Id = Convert.ToInt32(game.id),
                                  Name = game.name,
                                  ImageUrl = new Uri(game.logo, UriKind.Absolute),
                                  StatsUrl = new Uri(game.statsLink, UriKind.Absolute),
                                  StoreUrl = new Uri(game.storeLink, UriKind.Absolute)
                              };
        }

        #endregion
    }
}