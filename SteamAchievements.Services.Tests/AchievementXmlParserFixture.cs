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
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AchievementXmlParserFixture
    {
        [Test]
        public void ParseClosedValid()
        {
            string xml = File.ReadAllText("achievements.xml");

            AchievementXmlParser parser = new AchievementXmlParser();
            IEnumerable<UserAchievement> achievements = parser.ParseClosed(xml);

            Assert.That(achievements.Any());
            Assert.That(achievements.All(a => a.SteamUserId == "nullreference"));
            Assert.That(achievements.Any(a => a.Name == "Fried Piper" && a.Closed));
            Assert.That(!achievements.Any(a => !a.Closed));
            Assert.That(achievements.Any(a => a.Name == "Midnight Rider" && a.Closed && a.Date.Ticks == 1261871228));
        }

        [Test]
        public void ParseHl2Achievements()
        {
            string xml = File.ReadAllText("hl2Achievements.xml");

            AchievementXmlParser parser = new AchievementXmlParser();
            parser.Parse(xml);
        }

        [Test]
        public void ParseNotValid()
        {
            AchievementXmlParser parser = new AchievementXmlParser();

            Assert.Throws<XmlException>(() => parser.Parse("asdf asdf asdf asdf"));
        }

        [Test]
        public void ParseValid()
        {
            string xml = File.ReadAllText("achievements.xml");

            AchievementXmlParser parser = new AchievementXmlParser();
            IEnumerable<UserAchievement> achievements = parser.Parse(xml);

            Assert.That(achievements.Any());
            Assert.That(achievements.All(a => a.SteamUserId == "nullreference"));
            Assert.That(achievements.Any(a => a.Name == "Fried Piper" && a.Closed));
            Assert.That(achievements.Any(a => a.Name == "Cl0wnd" && !a.Closed));
        }
    }
}