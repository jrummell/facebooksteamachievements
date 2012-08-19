#region License

//  Copyright 2012 John Rummell
//  
//  This file is part of SteamAchievements.
//  
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
//  
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

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
        ///   Prepares the log.
        /// </summary>
        /// <param name="publisher"> The publisher. </param>
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
            var communityManager = new SteamCommunityManager(new WebClientWrapper(), new SteamProfileXmlParser(),
                                                             new GameXmlParser(), new AchievementXmlParser(), logger);
            var achievementService = new AchievementService(achievementManager, communityManager);
            var facebookClient = new FacebookClientService(settings.FacebookAppId, settings.FacebookAppSecret,
                                                           settings.FacebookCanvasUrl);
            var autoUpdateManager = new AutoUpdateManager(achievementService, new UserService(achievementManager),
                                                          facebookClient, logger);
            return new Publisher(autoUpdateManager);
        }
    }
}