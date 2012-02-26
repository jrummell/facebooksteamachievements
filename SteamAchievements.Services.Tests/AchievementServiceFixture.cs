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
using System.Data.Linq;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AchievementServiceFixture
    {
    	[TestFixtureSetUp]
    	public void TestFixtureSetUp()
    	{
    		ModelMapCreator mapCreator = new ModelMapCreator();
    		mapCreator.CreateMappings();
    	}
    	
        [Test]
        public void UpdateNewUserAchievements()
        {
            Mock<IAchievementManager> achievementManagerMock = new Mock<IAchievementManager>();
            Mock<ISteamCommunityManager> communityManagerMock = new Mock<ISteamCommunityManager>();

            // expect
            Models.User user = new Models.User {FacebookUserId = 1234, SteamUserId = "user1"};
            Data.User dataUser = new Data.User {FacebookUserId = 1234, SteamUserId = "user1"};
            achievementManagerMock.Setup(rep => rep.GetUser(user.FacebookUserId))
                .Returns(dataUser).Verifiable();

            AchievementXmlParser achievementXmlParser = new AchievementXmlParser();
            List<Models.UserAchievement> userAchievements =
                achievementXmlParser.ParseClosed(File.ReadAllText("cssAchievements.xml")).ToList();
            userAchievements.ForEach(
                userAchievement =>
                userAchievement.Achievement.Game =
                new Game
                    {
                        Id = 240,
                        ImageUrl =
                            "http://media.steampowered.com/steamcommunity/public/images/apps/10/af890f848dd606ac2fd4415de3c3f5e7a66fcb9f.jpg",
                        Name = "Counter-Strike: Source",
                        PlayedRecently = true,
                        StatsUrl =
                            String.Format("http://steamcommunity.com/id/{0}/games/?xml=1", user.SteamUserId),
                        StoreUrl = "http://store.steampowered.com/app/10"
                    });

            communityManagerMock.Setup(rep => rep.GetClosedAchievements(user.SteamUserId, "english"))
                .Returns(new List<Models.UserAchievement>()).Verifiable();

            achievementManagerMock.Setup(rep => rep.GetUser(user.FacebookUserId))
                .Returns(dataUser).Verifiable();
            achievementManagerMock.Setup(rep => rep.UpdateAchievements(It.IsAny<IEnumerable<Data.UserAchievement>>()))
                .Returns(5).Verifiable();

            ICollection<Game> games = new GameXmlParser().Parse(File.ReadAllText("games.xml"));
            communityManagerMock.Setup(rep => rep.GetGames(user.SteamUserId, "english"))
                .Returns(games).Verifiable();

            Data.Achievement[] dataAchievements = new[]
                                                      {
                                                          new Data.Achievement
                                                              {
                                                                  AchievementNames =
                                                                      new EntitySet<AchievementName>
                                                                          {
                                                                              new AchievementName
                                                                                  {
                                                                                      Name = "x",
                                                                                      Description = "y",
                                                                                      Language = "english"
                                                                                  }
                                                                          },
                                                                  GameId = 1,
                                                                  Id = 1
                                                              }
                                                      };
            achievementManagerMock.Setup(
                rep => rep.GetUnpublishedAchievements(user.FacebookUserId, DateTime.UtcNow.Date.AddDays(-2)))
                .Returns(dataAchievements).Verifiable();
            achievementManagerMock.Setup(
                rep =>
                rep.UpdateHidden(user.FacebookUserId, It.IsAny<IEnumerable<int>>()))
                .Verifiable();

            // execute
            IAchievementService service =
                new AchievementService(achievementManagerMock.Object, communityManagerMock.Object);
            service.UpdateNewUserAchievements(user);

            // verify
            achievementManagerMock.Verify();
            communityManagerMock.Verify();
        }
    }
}