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
using System.Web.Configuration;

namespace SteamAchievements
{
    public partial class PublishDialogTest : System.Web.UI.Page
    {
        protected string FacebookClientId
        {
            get { return WebConfigurationManager.AppSettings["APIKey"]; }
        }

        protected string FacebookCallbackUrl
        {
            get { return WebConfigurationManager.AppSettings["Callback"]; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (String.IsNullOrEmpty(Request["access_token"]))
            {
                string redirectUrl = "https://graph.facebook.com/oauth/authorize?client_id="
                                     + FacebookClientId + "&redirect_uri=" + FacebookCallbackUrl +
                                     "&type=user_agent&display=popup";

                Response.Redirect(redirectUrl);
            }
        }
    }
}