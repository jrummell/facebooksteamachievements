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
                                              new Game {Id = 1, Name = "Left 4 Dead", Abbreviation = "l4d"},
                                              new Game {Id = 2, Name = "Left 4 Dead 2", Abbreviation = "l4d2"}
                                          };
            IEnumerable<Achievement> achievements = manager.GetAchievements("nullreference", games);
            Assert.That(achievements.Any());

            Assert.That(achievements.SingleOrDefault(a => a.Name == "Acid Reflex"), Is.Not.Null);
        }

        [Test]
        public void GetGameAchievements()
        {
            SteamCommunityManager manager = new SteamCommunityManager();

            Game game = new Game { Id = 1, Name = "Left 4 Dead", Abbreviation = "l4d" };

            IEnumerable<Achievement> achievements = manager.GetAchievements("nullreference", game);

            Assert.That(achievements.Any());
            Assert.That(achievements.All(a => a.GameId == game.Id));
        }
    }
}