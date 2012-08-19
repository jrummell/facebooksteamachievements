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
using System.Web;
using System.Web.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Controllers
{
    /// <summary>
    ///   Parses the signed_request parameter and sets the user session value.
    /// </summary>
    public class CanvasSignedRequestAttribute : ActionFilterAttribute
    {
        private const string _userSettingsKey = "UserSettings";
        private readonly IErrorLogger _errorLogger;
        private readonly IFacebookClientService _facebookClient;
        private readonly IUserService _userService;

        public CanvasSignedRequestAttribute(IFacebookClientService facebookClient, IUserService userService,
                                            IErrorLogger errorLogger)
        {
            _facebookClient = facebookClient;
            _userService = userService;
            _errorLogger = errorLogger;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase context = filterContext.HttpContext;
            string signedRequestValue = context.Request["signed_request"];
            HttpSessionStateBase session = context.Session;

            if (String.IsNullOrEmpty(signedRequestValue))
            {
                _errorLogger.Log(new InvalidOperationException("Missing signed_request."));

                return;
            }

            if (session[_userSettingsKey] == null)
            {
                SignedRequest signedRequest = _facebookClient.ParseSignedRequest(signedRequestValue);

                User user = _userService.GetUser(signedRequest.UserId);
                if (user == null)
                {
                    user = new User {FacebookUserId = signedRequest.UserId};
                }

                user.AccessToken = signedRequest.AccessToken;
                user.SignedRequest = signedRequestValue;

                session[_userSettingsKey] = user;
            }
        }
    }
}