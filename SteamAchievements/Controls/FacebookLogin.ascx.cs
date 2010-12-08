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
using System.Web.UI;
using Facebook;
using Facebook.Web;
using SteamAchievements.Data;

namespace SteamAchievements.Controls
{
    public partial class FacebookLogin : UserControl
    {
        private FacebookApp _facebookApp;
        private CanvasAuthorizer _authorizer;

        public FacebookLogin()
        {
            _facebookApp = new FacebookApp();
            _authorizer = new CanvasAuthorizer(_facebookApp);
            _authorizer.Perms = "publish_stream,offline_access";
        }

        public string AccessToken
        {
            get { return (string)ViewState["AccessToken"] ?? String.Empty; }
            private set { ViewState["AccessToken"] = value; }
        }

        protected string FacebookClientId
        {
            get { return FacebookSettings.Current.ApiKey; }
        }

        public long FacebookUserId
        {
            get { return (long)(ViewState["FacebookUserId"] ?? 0); }
            private set { ViewState["FacebookUserId"] = value; }
        }

        public bool IsLoggedIn
        {
            get { return (bool)(ViewState["IsLoggedIn"] ?? false); }
            private set { ViewState["IsLoggedIn"] = value; }
        }

        public string SteamUserId
        {
            get { return (string)ViewState["SteamUserId"] ?? String.Empty; }
            private set { ViewState["SteamUserId"] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IsLoggedIn = _authorizer.IsAuthorized();

            if (!IsLoggedIn)
            {
                Uri authurl = _authorizer.GetLoginUrl(new HttpRequestWrapper(Request));
                CanvasRedirect(authurl.ToString());
            }
            else
            {
                FacebookUserId = _facebookApp.UserId;
                AccessToken = _facebookApp.Session.AccessToken;

                using (IAchievementManager manager = new AchievementManager())
                {
                    User user = manager.GetUser(FacebookUserId);
                    if (user != null)
                    {
                        SteamUserId = user.SteamUserId;
                    }
                }
            }
        }

        private void CanvasRedirect(string url)
        {
            string content = CanvasUrlBuilder.GetCanvasRedirectHtml(url);

            Response.ContentType = "text/html";
            Response.Write(content);
        }
    }
}