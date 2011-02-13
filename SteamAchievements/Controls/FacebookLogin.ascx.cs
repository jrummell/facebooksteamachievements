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
using System.Collections.Generic;
using System.Web.UI;
using Facebook.Web;
using SteamAchievements.Services;

namespace SteamAchievements.Controls
{
    public partial class FacebookLogin : UserControl
    {
        private readonly CanvasAuthorizer _authorizer;

        public FacebookLogin()
        {
            _authorizer = new CanvasAuthorizer {Perms = "publish_stream,offline_access"};
        }

        public string AccessToken
        {
            get { return (string) ViewState["AccessToken"] ?? String.Empty; }
            private set { ViewState["AccessToken"] = value; }
        }

        protected string FacebookClientId
        {
            get { return _authorizer.AppId; }
        }

        public long FacebookUserId
        {
            get { return (long) (ViewState["FacebookUserId"] ?? 0); }
            private set { ViewState["FacebookUserId"] = value; }
        }

        public bool IsLoggedIn
        {
            get { return (bool) (ViewState["IsLoggedIn"] ?? false); }
            private set { ViewState["IsLoggedIn"] = value; }
        }

        public string SteamUserId
        {
            get { return (string) ViewState["SteamUserId"] ?? String.Empty; }
            private set { ViewState["SteamUserId"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IsLoggedIn = _authorizer.IsAuthorized();

            if (!IsLoggedIn)
            {
                Dictionary<string, object> parameters =
                    new Dictionary<string, object>
                        {
                            {"scope", _authorizer.Perms},
                            {"client_id", _authorizer.AppId},
                            {"redirect_uri", _authorizer.ReturnUrlPath},
                            {"response_type", "token"}
                        };
                Uri authurl = _authorizer.GetLoginUrl(parameters);
                CanvasRedirect(authurl);
            }
            else
            {
                FacebookUserId = Convert.ToInt64(_authorizer.Session.UserId);
                AccessToken = _authorizer.Session.AccessToken;

                using (IUserService manager = new UserService())
                {
                    User user = manager.GetUser(FacebookUserId);
                    if (user != null)
                    {
                        SteamUserId = user.SteamUserId;
                    }
                }
            }
        }

        private void CanvasRedirect(Uri url)
        {
            string content = CanvasUrlBuilder.GetCanvasRedirectHtml(url);

            Response.ContentType = "text/html";
            Response.Write(content);
        }
    }
}