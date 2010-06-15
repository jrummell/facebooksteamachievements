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

namespace SteamAchievements.Services.Tests
{
    [TestFixture, Explicit("Requires internet connection")]
    public class SteamCommunityManagerFixture
    {
        [Test]
        public void GetAchievements()
        {
            SteamCommunityManager manager = new SteamCommunityManager();

            IEnumerable<Achievement> achievements = manager.GetAchievements("nullreference");

            Assert.That(achievements.Any());
            Assert.That(achievements.Any(a => a.Name == "Acid Reflex"));
        }

        [Test]
        public void GetGames()
        {
            SteamCommunityManager manager = new SteamCommunityManager();

            IEnumerable<Game> games = manager.GetGames("nullreference");

            Assert.That(games.Any());
            Assert.That(games.Any(game => game.Name == "Left 4 Dead"));
            Assert.That(games.Any(game => game.Name == "Left 4 Dead 2"));
        }
    }
}