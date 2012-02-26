using System;
using System.IO;
using Microsoft.Practices.Unity;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Updater
{
    internal class Program
    {
        private static IUnityContainer _container;
        private static readonly DirectoryInfo _logDirectory = new DirectoryInfo("logs");

        private static void Main(string[] args)
        {
            _container = BuildContainer();

            ModelMapCreator mapCreator = new ModelMapCreator();
            mapCreator.CreateMappings();

            Publisher publisher = _container.Resolve<Publisher>();

            PrepareLog(publisher);

            try
            {
                publisher.Publish();
            }
            finally
            {
                publisher.Dispose();
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

        private static IUnityContainer BuildContainer()
        {
            UnityContainer container = new UnityContainer();

            container.RegisterType<ISteamCommunityManager, SteamCommunityManager>();
            container.RegisterType<IWebClientWrapper, WebClientWrapper>();
            container.RegisterType<ISteamProfileXmlParser, SteamProfileXmlParser>();
            container.RegisterType<IGameXmlParser, GameXmlParser>();
            container.RegisterType<IAchievementXmlParser, AchievementXmlParser>();

            container.RegisterType<IAchievementService, AchievementService>();
            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IAchievementManager, AchievementManager>();
            container.RegisterType<ISteamRepository, SteamRepository>();

            container.RegisterType<IAutoUpdateLogger, AutoUpdateLogger>(new InjectionConstructor(_logDirectory.FullName));
            container.RegisterType<IAutoUpdateManager, AutoUpdateManager>();
            container.RegisterType<IFacebookPublisher, FacebookPublisher>();

            container.RegisterType<Publisher>();

            return container;
        }
    }
}