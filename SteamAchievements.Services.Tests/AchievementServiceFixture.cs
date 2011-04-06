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
using System.Xml;
using Moq;
using NUnit.Framework;
using NUnit.Mocks;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AchievementServiceFixture
    {
        /// <summary>
        /// Gets the community achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        /// <returns></returns>
        private static ICollection<Data.UserAchievement> GetCommunityAchievements(string steamUserId)
        {
            List<UserAchievement> achievements;
            using (FileStream fs = new FileStream("Serialized" + steamUserId + "Achievements.xml", FileMode.Open))
            {
                using (XmlDictionaryReader reader =
                    XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
                {
                    DataContractSerializer serializer =
                        new DataContractSerializer(typeof (UserAchievement),
                                                   new[] {typeof (List<UserAchievement>)});

                    achievements = (List<UserAchievement>) serializer.ReadObject(reader, true);
                }
            }

            return achievements.ToDataAchievements(0);
        }

        /// <summary>
        /// Gets the data achievements.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Achievement> GetDataAchievements()
        {
            string text = File.ReadAllText("achievements.csv");

            List<Achievement> achievements = new List<Achievement>();
            foreach (string line in text.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                // Id	Name	GameId	Description	ImageUrl
                string[] fields = line.Split('\t');

                Achievement achievement = new Achievement
                                              {
                                                  Id = Convert.ToInt32(fields[0]),
                                                  Name = fields[1],
                                                  GameId = Convert.ToInt32(fields[2]),
                                                  Description = fields[3],
                                                  ImageUrl = fields[4]
                                              };

                achievements.Add(achievement);
            }
            return achievements;
        }

        /// <summary>
        /// Updates the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        private static void UpdateAchievements(string steamUserId)
        {
            DynamicMock repositoryMock = new DynamicMock(typeof (ISteamRepository));
            ISteamRepository repository = (ISteamRepository) repositoryMock.MockInstance;
            repositoryMock.ExpectAndReturn("get_Achivements", GetDataAchievements().AsQueryable());
            repositoryMock.ExpectAndReturn("get_Users",
                                           new List<Data.User>
                                               {new Data.User {FacebookUserId = 0, SteamUserId = steamUserId}}.
                                               AsQueryable());
            repositoryMock.ExpectAndReturn("get_UserAchievements", new List<Data.UserAchievement>().AsQueryable());

            AchievementManager manager = new AchievementManager(repository);
            ICollection<Data.UserAchievement> achievements = GetCommunityAchievements(steamUserId);

            // should not throw InvalidOperationException
            manager.UpdateAchievements(achievements);
        }

        /// <summary>
        /// Serializes the achievements.
        /// </summary>
        /// <param name="steamUserId">The steam user id.</param>
        private static void SerializeAchievements(string steamUserId)
        {
            SteamCommunityManager manager = new SteamCommunityManager();
            List<UserAchievement> achievements = manager.GetAchievements(steamUserId).ToList();

            FileInfo dll = new FileInfo("SteamAchievements.Services.Tests");
            DirectoryInfo bin = new DirectoryInfo(dll.Directory.FullName);
            string projectPath = bin.Parent.Parent.FullName;
            string serializedFilePath = Path.Combine(projectPath, "Serialized" + steamUserId + "Achievements.xml");

            DataContractSerializer serializer =
                new DataContractSerializer(typeof (UserAchievement), new[] {typeof (List<UserAchievement>)});
            using (FileStream writer = new FileStream(serializedFilePath, FileMode.Create))
            {
                serializer.WriteObject(writer, achievements);
            }
        }

        /// <summary>
        /// http://code.google.com/p/facebooksteamachievements/issues/detail?id=37
        /// </summary>
        [Test, Explicit]
        public void Issue37()
        {
            UpdateAchievements("Unimatrixero");
        }

        /// <summary>
        /// http://code.google.com/p/facebooksteamachievements/issues/detail?id=51
        /// </summary>
        [Test, Explicit]
        public void Issue51()
        {
            UpdateAchievements("richardmstallman");
        }

        [Test, Explicit]
        public void SerializeAchievementsForIssue37()
        {
            SerializeAchievements("Unimatrixero");
        }

        [Test, Explicit]
        public void SerializeAchievementsForIssue51()
        {
            SerializeAchievements("richardmstallman");
        }

        [Test]
        public void UpdateNewUserAchievements()
        {
            Mock<IAchievementManager> achievementManagerMock = new Mock<IAchievementManager>();
            Mock<ISteamCommunityManager> communityManagerMock = new Mock<ISteamCommunityManager>();

            // expect
            User user = new User {FacebookUserId = 1234, SteamUserId = "user1"};
            Data.User dataUser = new Data.User {FacebookUserId = 1234, SteamUserId = "user1"};
            achievementManagerMock.Setup(rep => rep.GetUser(user.FacebookUserId))
                .Returns(dataUser).Verifiable();

            AchievementXmlParser achievementXmlParser = new AchievementXmlParser();
            List<UserAchievement> userAchievements =
                achievementXmlParser.ParseClosed(File.ReadAllText("cssAchievements.xml")).ToList();
            userAchievements.ForEach(
                userAchievement =>
                userAchievement.Game =
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

            communityManagerMock.Setup(rep => rep.GetAchievements(user.SteamUserId))
                .Returns(new List<UserAchievement>()).Verifiable();

            achievementManagerMock.Setup(rep => rep.GetUser(user.SteamUserId))
                .Returns(dataUser).Verifiable();
            achievementManagerMock.Setup(rep => rep.UpdateAchievements(It.IsAny<IEnumerable<Data.UserAchievement>>()))
                .Returns(5).Verifiable();

            ICollection<Game> games = new GameXmlParser().Parse(File.ReadAllText("games.xml"));
            communityManagerMock.Setup(rep => rep.GetGames(user.SteamUserId))
                .Returns(games).Verifiable();

            Achievement[] dataAchievements = new[] {new Achievement {Description = "x", GameId = 1, Id = 1,}};
            achievementManagerMock.Setup(
                rep => rep.GetUnpublishedAchievements(user.SteamUserId, DateTime.UtcNow.Date.AddDays(-2)))
                .Returns(dataAchievements).Verifiable();
            achievementManagerMock.Setup(
                rep =>
                rep.UpdateHidden(user.SteamUserId, It.IsAny<IEnumerable<int>>()))
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