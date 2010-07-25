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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using NUnit.Framework;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class GameXmlParserFixture
    {
        [Test]
        public void ParseNotValid()
        {
            GameXmlParser parser = new GameXmlParser();

            Assert.Throws<XmlException>(() => parser.Parse("asdf asdf asdf asdf"));
        }

        [Test]
        public void ParseValid()
        {
            string xml = File.ReadAllText("games.xml");

            GameXmlParser parser = new GameXmlParser();
            IEnumerable<Game> games = parser.Parse(xml);

            Assert.That(games.Any());
            Assert.That(games.Count(game => game.PlayedRecently), Is.EqualTo(2));
        }

        [Test, Explicit]
        public void SerializeGames()
        {
            FileInfo file = new FileInfo("UnimatrixeroGames.xml");
            string xml = File.ReadAllText(file.FullName);

            GameXmlParser parser = new GameXmlParser();
            List<Game> games = parser.Parse(xml).ToList();

            Assert.That(games.Count, Is.GreaterThan(0));

            DataContractSerializer serializer = new DataContractSerializer(typeof (Game), new[] {typeof (List<Game>)});
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                serializer.WriteObject(writer, games);
            }

            string serializedGamesXml = builder.ToString();

            string solutionDirectoryPath = file.Directory.Parent.Parent.Parent.FullName;

            Console.WriteLine(solutionDirectoryPath);

            string dataTestProjectPath = Path.Combine(solutionDirectoryPath, "SteamAchievements.Data.Tests");
            string serializedGamesFileName = Path.Combine(dataTestProjectPath, "UnimatrixeroGameEntities.xml");

            File.WriteAllText(serializedGamesFileName, serializedGamesXml);
        }
    }
}