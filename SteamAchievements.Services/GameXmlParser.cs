using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace SteamAchievements.Services
{
    public class GameXmlParser : IXmlParser<Game>
    {
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
    }
}
