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

using Bootstrap.UnityExtension;
using Microsoft.Practices.Unity;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Bootstrapping
{
    public class UnityRegistration : IUnityRegistration
    {
        #region IUnityRegistration Members

        public void Register(IUnityContainer container)
        {
#if DEBUG
            LifetimeManager lifetimeManager = new ContainerControlledLifetimeManager();
            container.RegisterType<ISteamRepository, MockSteamRepository>(lifetimeManager);
            container.RegisterType<IFacebookContextSettings, MockFacebookContextSettings>();
#else
            container.RegisterType<ISteamRepository, SteamRepository>();
            container.RegisterType<IFacebookContextSettings, FacebookContextSettings>();
#endif

            container.RegisterType<IAchievementXmlParser, AchievementXmlParser>();
            container.RegisterType<IGameXmlParser, GameXmlParser>();
            container.RegisterType<ISteamProfileXmlParser, SteamProfileXmlParser>();
            container.RegisterType<IWebClientWrapper, WebClientWrapper>();

            container.RegisterType<IAchievementManager, AchievementManager>();
            container.RegisterType<IAchievementService, AchievementService>();
            container.RegisterType<ISteamCommunityManager, SteamCommunityManager>();
            container.RegisterType<IUserService, UserService>();
        }

        #endregion
    }
}