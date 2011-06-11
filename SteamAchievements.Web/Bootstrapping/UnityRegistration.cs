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

using Microsoft.Practices.Unity;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Bootstrapping
{
    public class UnityRegistration
    {
        private readonly IUnityContainer _container;

        public UnityRegistration(IUnityContainer container)
        {
            _container = container;
        }

        public void Register()
        {
#if DEBUG
            LifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
            _container.RegisterType<ISteamRepository, MockSteamRepository>(lifetimeManager);
            _container.RegisterType<IFacebookContextSettings, MockFacebookContextSettings>();
#else
            _container.RegisterType<ISteamRepository, SteamRepository>();
            _container.RegisterType<IFacebookContextSettings, FacebookContextSettings>();
#endif

            _container.RegisterType<IAchievementXmlParser, AchievementXmlParser>();
            _container.RegisterType<IGameXmlParser, GameXmlParser>();
            _container.RegisterType<ISteamProfileXmlParser, SteamProfileXmlParser>();
            _container.RegisterType<IWebClientWrapper, WebClientWrapper>();

            _container.RegisterType<IAchievementManager, AchievementManager>();
            _container.RegisterType<IAchievementService, AchievementService>();
            _container.RegisterType<ISteamCommunityManager, SteamCommunityManager>();
            _container.RegisterType<IUserService, UserService>();
        }
    }
}