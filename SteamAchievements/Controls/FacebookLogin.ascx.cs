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
using Facebook;
using Facebook.Web;
using SteamAchievements.Properties;

namespace SteamAchievements.Controls
{
    public partial class FacebookLogin : UserControl
    {
        protected string FacebookClientId
        {
            get { return FacebookSettings.Current.ApiKey; }
        }

        public bool IsLoggedIn
        {
            get; private set;
        }

        public long FacebookUserId { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["FacebookUserId"] == null)
            {
                FacebookApp facebookApp = new FacebookApp();
                CanvasAuthorizer authorizor = new CanvasAuthorizer(facebookApp);
                authorizor.ReturnUrlPath = "default.aspx";
                authorizor.Perms = "publish_stream"; //,offline_access
                authorizor.Authorize(Request, Response);
                IsLoggedIn = authorizor.IsAuthorized();

                if (IsLoggedIn)
                {
                    Session["FacebookUserId"] = facebookApp.UserId;
                }
                else
                {
                    Session["FacebookUserId"] = 0;
                }
            }

            FacebookUserId = (long) Session["FacebookUserId"];
            IsLoggedIn = FacebookUserId > 0;
        }
    }
}