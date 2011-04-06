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
using System.Linq;
using System.Xml.Linq;

namespace SteamAchievements.Services
{
    public class SteamProfileXmlParser : IXmlParser<SteamProfile>
    {
        #region IXmlParser<SteamProfile> Members

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public ICollection<SteamProfile> Parse(string xml)
        {
            XDocument document = XDocument.Parse(xml);

            var profiles =
                from element in document.Descendants("profile")
                let steamID = element.Element("steamID")
                let avatarMedium = element.Element("avatarMedium")
                let headline = element.Element("headline")
                where steamID != null && avatarMedium != null && headline != null
                select new
                           {
                               steamId = steamID.Value,
                               avatarMedium = avatarMedium.Value,
                               headline = headline.Value
                           };

            return (from profile in profiles
                    select new SteamProfile
                               {
                                   SteamUserId = profile.steamId,
                                   AvatarUrl = profile.avatarMedium,
                                   Headline = profile.headline
                               }).ToList();
        }

        #endregion

        /// <summary>
        /// Parses the profile XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public SteamProfile ParseProfile(string xml)
        {
            return Parse(xml).SingleOrDefault();
        }
    }
}