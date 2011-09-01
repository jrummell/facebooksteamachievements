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
using System.Web;
using System.Web.Mvc;
using Facebook;
using Facebook.Web;

namespace SteamAchievements.Web.Models
{
    public class FacebookContextSettings : IFacebookContextSettings
    {
        public FacebookContextSettings()
        {
            IFacebookApplication settings = FacebookApplication.Current;
            if (settings != null)
            {
                CanvasPage = settings.CanvasPage;
                AppId = settings.AppId;
            }

            FacebookWebContext facebookContext = FacebookWebContext.Current;
            FacebookSignedRequest signedRequest = facebookContext.SignedRequest;

            if (settings != null && signedRequest == null)
            {
                signedRequest = ParseSignedRequest(settings);
            }

            if (signedRequest != null)
            {
                AccessToken = signedRequest.AccessToken;
                UserId = signedRequest.UserId;
            }
        }

        private FacebookSignedRequest ParseSignedRequest(IFacebookApplication settings)
        {
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                HttpContextWrapper contextWrapper = new HttpContextWrapper(context);
                if (contextWrapper.Request.IsAjaxRequest())
                {
                    // ajax requests won't have a signed request, so we need to build it from the current http request
                    // see http://facebooksdk.codeplex.com/discussions/251878

                    try
                    {
                        return FacebookSignedRequest.Parse(settings, SignedRequest);
                    }
                    catch (Exception exception)
                    {
                        // Facebook posts to the iframe, but only IE supports this so the first request will always fail for non IE browsers
                        if (context.Request.Browser.Browser.Contains("IE"))
                        {
                            throw;
                        }

                        // it doesn't break anything so we'll throw a custom exception so that we can filter it out later
                        InvalidSignedRequestException signedRequestException =
                            new InvalidSignedRequestException(
                                "Invalid SignedRequest - Non - IE (" + SignedRequest + ")",
                                exception);
                        throw signedRequestException;
                    }
                }
            }

            return null;
        }

        #region IFacebookContextSettings Members

        public string CanvasPage { get; private set; }

        public string AccessToken { get; private set; }

        public string AppId { get; private set; }

        public long UserId { get; private set; }

        public string SignedRequest
        {
            get { return HttpContext.Current.Request.Params["signed_request"]; }
        }

        #endregion
    }
}