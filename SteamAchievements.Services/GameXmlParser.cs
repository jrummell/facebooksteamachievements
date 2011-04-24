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
    public class GameXmlParser : IGameXmlParser
    {
        #region IXmlParser<Game> Members

        /// <summary>
        /// Returns a collection of <see cref="Game"/>s from the given xml.
        /// </summary>
        public ICollection<Game> Parse(string xml)
        {
            XDocument document = XDocument.Parse(xml);

            var games =
                from element in document.Descendants("game")
                let hoursLast2Weeks = element.Element("hoursLast2Weeks")
                let storeLink = element.Element("storeLink")
                let statsLink = element.Element("statsLink")
                let logo = element.Element("logo")
                let name = element.Element("name")
                let appID = element.Element("appID")
                where
                    statsLink != null && storeLink != null && logo != null && name != null && appID != null
                select new
                           {
                               id = appID.Value,
                               name = name.Value,
                               logo = logo.Value,
                               statsLink = statsLink.Value,
                               storeLink = storeLink.Value,
                               playedRecently = hoursLast2Weeks != null
                           };

            return (from game in games
                    select new Game
                               {
                                   Id = Convert.ToInt32(game.id),
                                   Name = game.name,
                                   ImageUrl = game.logo,
                                   StatsUrl = game.statsLink,
                                   StoreUrl = game.storeLink,
                                   PlayedRecently = game.playedRecently
                               }).ToList();
        }

        #endregion
    }
}