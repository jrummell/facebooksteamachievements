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
using System.IO;
using SteamAchievements.Data;
using SteamAchievements.Services;

namespace SteamAchievements.Updater
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (Publisher publisher = new Publisher(GetAutoUpdateManager()))
            {
                publisher.Publish();
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