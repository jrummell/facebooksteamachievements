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
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    /// <summary>
    ///   Parses the signed_request parameter and sets the user session value.
    /// </summary>
    public class CanvasSignedRequestAttribute : SignedRequestAttribute
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
            HttpSessionStateBase session = context.Session;

            if (session != null && session[_userSettingsKey] == null)
            {
                User user = GetUser(context);

                session[_userSettingsKey] = user;
            }
        }

        protected override User GetUser(HttpContextBase context)
        {
            string signedRequestValue = context.Request["signed_request"];
            try
            {
                ValidateSignedRequest(signedRequestValue);
            }
            catch (Exception ex)
            {
                _errorLogger.Log(ex);
                return null;
            }

            SignedRequest signedRequest = _facebookClient.ParseSignedRequest(signedRequestValue);

            User user = _userService.GetUser(signedRequest.UserId);
            if (user == null)
            {
                user = new User {FacebookUserId = signedRequest.UserId};
            }

            user.AccessToken = signedRequest.AccessToken;
            user.SignedRequest = signedRequestValue;

            return user;
        }

        private void ValidateSignedRequest(string signedRequest)
        {
            if (String.IsNullOrEmpty(signedRequest))
            {
                string message = String.Format("Invalid signed request : \"{0}\"", signedRequest);
                throw new InvalidSignedRequestException(message);
            }
        }
    }
}