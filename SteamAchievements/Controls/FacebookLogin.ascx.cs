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
using System.Web.UI;
using Microsoft.Practices.Unity;
using SteamAchievements.Services;

namespace SteamAchievements.Controls
{
    public partial class FacebookLogin : UserControl
    {
        private readonly ICanvasAuthorizer _authorizer;
        private readonly IUnityContainer _container;

        protected FacebookLogin()
            : this(null)
        {
        }

        protected FacebookLogin(ICanvasAuthorizer authorizer)
        {
            _container = ContainerManager.Container;
            _authorizer = authorizer ?? _container.Resolve<ICanvasAuthorizer>();
        }

        protected string FacebookClientId
        {
            get { return _authorizer.AppId; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            bool loggedIn = _authorizer.IsAuthorized();

            if (!loggedIn)
            {
                _authorizer.HandleUnauthorizedRequest();
                return;
            }

            long facebookUserId = Convert.ToInt64(_authorizer.UserId);

            string steamUserId = null;
            bool publishDescription = false;
            using (IUserService manager = _container.Resolve<IUserService>())
            {
                User user = manager.GetUser(facebookUserId);
                if (user != null)
                {
                    steamUserId = user.SteamUserId;
                    publishDescription = user.PublishDescription;
                }
            }

            Session[FacebookPage.FacebookSettingsCacheKey] =
                new FacebookSettings
                    {
                        FacebookUserId = facebookUserId,
                        SteamUserId = steamUserId,
                        AccessToken = _authorizer.AccessToken,
                        PublishDescription = publishDescription
                    };
        }
    }
}