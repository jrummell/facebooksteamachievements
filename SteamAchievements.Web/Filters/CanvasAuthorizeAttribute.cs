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

namespace SteamAchievements.Web.Filters
{
    public class CanvasAuthorizeAttribute : AuthorizeAttribute
    {
        public const string UserSettingsKey = "UserSettings";

        private readonly IFacebookClientService _facebookClient;
        private readonly IFormsAuthenticationService _formsAuthenticationService;
        private readonly IUserService _userService;

        public CanvasAuthorizeAttribute(IFacebookClientService facebookClient, IUserService userService,
                                        IFormsAuthenticationService formsAuthenticationService)
        {
            _facebookClient = facebookClient;
            _userService = userService;
            _formsAuthenticationService = formsAuthenticationService;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session != null)
            {
                User user = (User) httpContext.Session[UserSettingsKey];
                if (user == null)
                {
                    user = GetUser(httpContext);

                    httpContext.Session[UserSettingsKey] = user;
                }

                if (user != null && user.FacebookUserId > 0)
                {
                    // sign the user in
                    _formsAuthenticationService.SignIn(user.FacebookUserId.ToString());

                    return true;
                }
            }

            return base.AuthorizeCore(httpContext);
        }

        private User GetUser(HttpContextBase context)
        {
            string signedRequestValue = context.Request["signed_request"];
            try
            {
                ValidateSignedRequest(signedRequestValue);
            }
            catch (Exception ex)
            {
                //_errorLogger.Log(ex);
                return null;
            }

            // parse the signed request
            SignedRequest signedRequest = _facebookClient.ParseSignedRequest(signedRequestValue);

            // get the user
            User user = _userService.GetUser(signedRequest.UserId);
            if (user == null)
            {
                user = new User {FacebookUserId = signedRequest.UserId};
            }

            // update access token and signed request
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