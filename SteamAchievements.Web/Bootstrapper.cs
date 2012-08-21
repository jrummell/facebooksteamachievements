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

using System.Web.Mvc;
using Microsoft.Practices.Unity;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Controllers;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Properties;
using Unity.Mvc3;

namespace SteamAchievements.Web
{
    public static class Bootstrapper
    {
        public static void Initialize()
        {
            IUnityContainer container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            ModelMapCreator mapCreator = new ModelMapCreator();
            mapCreator.CreateMappings();

            ViewModelMapCreator viewModelMapCreator = new ViewModelMapCreator();
            viewModelMapCreator.CreateMappings();
        }

        private static IUnityContainer BuildUnityContainer()
        {
            Settings settings = Settings.Default;
            UnityContainer container = new UnityContainer();

            if (settings.Mode == FacebookMode.None)
            {
                container.RegisterType<ISteamRepository, MockSteamRepository>(new HierarchicalLifetimeManager());
                container.RegisterType<IFacebookClientService, MockFacebookClientService>();
                container.RegisterType<IUserService, MockUserService>();
                container.RegisterType<SignedRequestAttribute, MockSignedRequestAttribute>();
                container.RegisterType<IAchievementService, MockAchievementService>();
            }
            else
            {
                container.RegisterType<ISteamRepository, SteamRepository>(new HierarchicalLifetimeManager());
                container.RegisterType<IFacebookClientService, FacebookClientService>(
                    new InjectionConstructor(settings.FacebookAppId, settings.FacebookAppSecret,
                                             settings.FacebookCanvasUrl));
                container.RegisterType<IUserService, UserService>();
                container.RegisterType<SignedRequestAttribute, CanvasSignedRequestAttribute>();
                container.RegisterType<IAchievementService, AchievementService>();
            }

            container.RegisterType<ISteamCommunityManager, SteamCommunityManager>();
            container.RegisterType<IAchievementXmlParser, AchievementXmlParser>();
            container.RegisterType<IGameXmlParser, GameXmlParser>();
            container.RegisterType<ISteamProfileXmlParser, SteamProfileXmlParser>();
            container.RegisterType<IWebClientWrapper, WebClientWrapper>(new HierarchicalLifetimeManager());
            container.RegisterType<IErrorLogger, ElmahErrorLogger>();

            container.RegisterType<IAchievementManager, AchievementManager>();

            return container;
        }
    }
}