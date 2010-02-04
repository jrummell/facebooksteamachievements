using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class SteamCommunityManagerFixture
    {
        [Test]
        public void GetAchievements()
        {
            SteamCommunityManager manager = new SteamCommunityManager();

            IEnumerable<Game> games = new[]
                                          {
                                              new Game{Id = 1, Name = "Left 4 Dead", Abbreviation = "l4d"},
                                              new Game{Id = 2, Name = "Left 4 Dead 2", Abbreviation = "l4d2"}
                                          };
            IEnumerable<Achievement> achievements = manager.GetAchievements("nullreference", games);
            Assert.That(achievements.Any());

            Assert.That(achievements.SingleOrDefault(a => a.Name == "Acid Reflex"), Is.Not.Null);
        }
    }
}