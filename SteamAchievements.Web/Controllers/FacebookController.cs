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

using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    [ElmahHandleError]
    public abstract class FacebookController : Controller
    {
        private const string _userSettingsKey = "UserSettings";
        private readonly IFacebookContextSettings _facebookSettings;
        private readonly IUserService _userService;

        protected FacebookController(IUserService userService, IFacebookContextSettings facebookSettings)
        {
            _userService = userService;
            _facebookSettings = facebookSettings;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        protected IUnityContainer Container
        {
            get { return ContainerManager.Container; }
        }

        /// <summary>
        /// Gets the facebook user id.
        /// </summary>
        public long FacebookUserId
        {
            get { return _facebookSettings.UserId; } 
        }

        /// <summary>
        /// Gets the user settings.
        /// </summary>
        public User UserSettings
        {
            get { return Session[_userSettingsKey] as User; }
            set { Session[_userSettingsKey] = value; }
        }

        /// <summary>
        /// Gets the user service.
        /// </summary>
        public IUserService UserService
        {
            get { return _userService; }
        }

        /// <summary>
        /// Initializes data that might not be available when the constructor is called.
        /// </summary>
        /// <param name="requestContext">The HTTP context and route data.</param>
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewBag.FacebookClientId = _facebookSettings.AppId;

            if (UserSettings != null)
            {
                return;
            }

            if (_facebookSettings.UserId == 0)
            {
                return;
            }

            UserSettings = UserService.GetUser(_facebookSettings.UserId);

            if (UserSettings != null)
            {
                // update the user's access token if it changed
                if (UserSettings.AccessToken != _facebookSettings.AccessToken)
                {
                    UserSettings.AccessToken = _facebookSettings.AccessToken;

                    UserService.UpdateUser(UserSettings);
                }
            }
        }

        /// <summary>
        /// Releases unmanaged resources and optionally releases managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userService.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}