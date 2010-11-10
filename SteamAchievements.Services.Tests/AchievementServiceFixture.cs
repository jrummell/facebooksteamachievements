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
using NUnit.Framework;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AchievementServiceFixture
    {
        private static IEnumerable<Data.UserAchievement> GetCommunityAchievements(string steamUserId)
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

                    achievements = (List<UserAchievement>)serializer.ReadObject(reader, true);
                }
            }

            return achievements.ToDataAchievements();
        }

        private static List<Data.Achievement> GetDataAchievements()
        {
            string text = File.ReadAllText("achievements.csv");

            List<Data.Achievement> achievements = new List<Data.Achievement>();
            foreach (string line in text.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                // Id	Name	GameId	Description	ImageUrl
                string[] fields = line.Split('\t');

                Data.Achievement achievement = new Data.Achievement
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

        private void UpdateAchievements(string steamUserId)
        {
            MockSteamRepository repository = new MockSteamRepository();
            repository.Achievements = GetDataAchievements().AsQueryable();
            repository.Users =
                new List<User> {new User {FacebookUserId = 0, SteamUserId = steamUserId}}.AsQueryable();
            repository.UserAchievements = new List<Data.UserAchievement>().AsQueryable();

            AchievementManager manager = new AchievementManager(repository);
            IEnumerable<Data.UserAchievement> achievements = GetCommunityAchievements(steamUserId);

            // should not throw InvalidOperationException
            manager.UpdateAchievements(achievements);
        }

        private void SerializeAchievements(string steamUserId)
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
    }
}