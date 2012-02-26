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
using System.Web.Security;

using SteamAchievements.Services;
using SteamAchievements.Services.Models;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    public abstract class FacebookController : Controller
    {
        private const string _userSettingsKey = "UserSettings";
        private readonly IFacebookContextSettings _facebookSettings;
        private readonly IUserService _userService;

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookController"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="facebookSettings">The facebook settings.</param>
        protected FacebookController(IUserService userService, IFacebookContextSettings facebookSettings)
        {
            _userService = userService;
            _facebookSettings = facebookSettings;
        }

        /// <summary>
        /// Gets the facebook user id.
        /// </summary>
        protected long FacebookUserId
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
        protected IUserService UserService
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

            // these values are used in the FacebookInitPartial view
            ViewBag.FacebookClientId = _facebookSettings.AppId;
            ViewBag.SignedRequest = _facebookSettings.SignedRequest;

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

                // make sure the user is signed in so that User.Identity.Name is thier FacebookUserId
                string userName = _facebookSettings.UserId.ToString();
                if (!User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.SetAuthCookie(userName, false);
                }
                else if (User.Identity.Name != userName)
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.SetAuthCookie(userName, false);
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