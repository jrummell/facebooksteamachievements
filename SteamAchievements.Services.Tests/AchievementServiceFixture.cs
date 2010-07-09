using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using SteamAchievements.Data;
using System.Runtime.Serialization;
using System.Xml;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AchievementServiceFixture
    {
        [Test, Explicit]
        public void Issue37()
        {
            MockSteamRepository repository = new MockSteamRepository();
            repository.Achievements = GetDataAchievements().AsQueryable();
            repository.Users = new List<User> { new User{ FacebookUserId = 0, SteamUserId = "Unimatrixero"} }.AsQueryable();
            repository.UserAchievements = new List<UserAchievement>().AsQueryable();

            GameXmlParser gameXmlParser = new GameXmlParser();
            IEnumerable<Game> games = gameXmlParser.Parse(File.ReadAllText("UnimatrixeroGames.xml"));

            AchievementManager manager = new AchievementManager(repository);
            manager.UpdateAchievements("AchievementManager", GetCommunityAchievements());

            //TODO: Assert
        }

        private static IEnumerable<Data.Achievement> GetCommunityAchievements()
        {
            List<Services.Achievement> achievements;
            using (FileStream fs = new FileStream("SerializedUnimatrixeroAchievements.xml", FileMode.Open))
            using (XmlDictionaryReader reader =
                XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
            {
                DataContractSerializer serializer =
                    new DataContractSerializer(typeof(Services.Achievement), new[] { typeof(List<Services.Achievement>) });

                achievements = (List<Services.Achievement>)serializer.ReadObject(reader, true);
            }

            return from achievement in achievements
                   select new Data.Achievement
                       {
                           Name = achievement.Name,
                           Description = achievement.Description,
                           ImageUrl = achievement.ImageUrl.ToString(),
                           GameId = achievement.Game.Id
                       };
        }

        private static List<Data.Achievement> GetDataAchievements()
        {
            string text = File.ReadAllText("achievements.csv");

            List<Data.Achievement> achievements = new List<Data.Achievement>();
            foreach (string line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
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

        [Test, Explicit]
        public void SerializeAchievements()
        {
            SteamCommunityManager manager = new SteamCommunityManager();
            List<Services.Achievement> achievements = manager.GetAchievements("Unimatrixero").ToList();

            FileInfo dll = new FileInfo("SteamAchievements.Services.Tests");
            DirectoryInfo bin = new DirectoryInfo(dll.Directory.FullName);
            string projectPath = bin.Parent.Parent.FullName;
            string serializedFilePath = Path.Combine(projectPath, "SerializedUnimatrixeroAchievements.xml");

            DataContractSerializer serializer = 
                new DataContractSerializer(typeof(Services.Achievement), new[] { typeof(List<Services.Achievement>) });
            using (FileStream writer = new FileStream(serializedFilePath, FileMode.Create))
            {
                serializer.WriteObject(writer, achievements);
            }
        }
    }
}
