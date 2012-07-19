using System;
using System.IO;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Updater.Properties;

namespace SteamAchievements.Updater
{
    internal class Program
    {
        private static readonly DirectoryInfo _logDirectory = new DirectoryInfo("logs");

        private static void Main(string[] args)
        {
            ModelMapCreator mapCreator = new ModelMapCreator();
            mapCreator.CreateMappings();

            using (Publisher publisher = CreatePublisher())
            {
                PrepareLog(publisher);

                publisher.Publish();
            }
        }

        /// <summary>
        /// Prepares the log.
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        private static void PrepareLog(Publisher publisher)
        {
            IAutoUpdateLogger autoUpdateLogger = publisher.Logger;

            // only keeps logs for the past 10 days
            autoUpdateLogger.Delete(DateTime.Now.AddDays(-10));
            // the default writer saves output to a file, this will display it in the console as well
            autoUpdateLogger.Attach(Console.Out);
        }

        private static Publisher CreatePublisher()
        {
            var settings = Settings.Default;
            var logger = new AutoUpdateLogger(_logDirectory.FullName);
            var achievementManager = new AchievementManager(new SteamRepository());
            var communityManager = new SteamCommunityManager(new WebClientWrapper(), new SteamProfileXmlParser(), new GameXmlParser(), new AchievementXmlParser());
            var achievementService = new AchievementService(achievementManager, communityManager);
            var facebookClient = new FacebookClientService(settings.FacebookAppId, settings.FacebookAppSecret, settings.FacebookCanvasUrl);
            var autoUpdateManager = new AutoUpdateManager(achievementService, new UserService(achievementManager), facebookClient, logger);
            return new Publisher(autoUpdateManager);
        }
    }
}