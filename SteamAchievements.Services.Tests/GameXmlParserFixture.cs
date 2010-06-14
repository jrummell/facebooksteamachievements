using System.Xml;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        }
    }
}
