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
            publisher.Publish();
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

            DirectoryInfo logDirectory = new DirectoryInfo("logs");
            if (!logDirectory.Exists)
            {
                logDirectory.Create();
            }

            AutoUpdateLogger autoUpdateLogger = new AutoUpdateLogger(logDirectory.FullName);
            AutoUpdateManager autoUpdateManager =
                new AutoUpdateManager(achievementService, userService, new FacebookPublisher(),
                                      autoUpdateLogger);

            return autoUpdateManager;
        }
    }
}