using System;
using System.IO;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements.Updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Publisher publisher = new Publisher(GetAutoUpdateManager());

            try
            {
                publisher.Publish();
            }
            finally
            {
                publisher.Dispose();
            }
        }

        private static AutoUpdateManager GetAutoUpdateManager()
        {
            // build up AutoUpdateManager - use DI instead?
            SteamCommunityManager steamCommunityManager =
                new SteamCommunityManager(new WebClientWrapper(), new SteamProfileXmlParser(), new GameXmlParser(),
                                          new AchievementXmlParser());
            AchievementService achievementService =
                new AchievementService(
                    new AchievementManager(new SteamRepository()), steamCommunityManager);
            UserService userService = new UserService(new AchievementManager(new SteamRepository()));

            // verify the log path
            DirectoryInfo logDirectory = new DirectoryInfo("logs");
            if (!logDirectory.Exists)
            {
                logDirectory.Create();
            }

            IAutoUpdateLogger autoUpdateLogger = new AutoUpdateLogger(logDirectory.FullName);
            // only keeps logs for the past 10 days
            autoUpdateLogger.Delete(DateTime.Now.AddDays(-10));
            // the default writer saves output to a file, this will display it in the console as well
            autoUpdateLogger.Attach(Console.Out);

            AutoUpdateManager autoUpdateManager =
                new AutoUpdateManager(achievementService, userService, new FacebookPublisher(),
                                      autoUpdateLogger);

            return autoUpdateManager;
        }
    }
}