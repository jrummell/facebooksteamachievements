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
using System.Web.Configuration;
using System.Web.UI;
using Facebook.Schema;
using Facebook.Web;
using SteamAchievements.Properties;

namespace SteamAchievements
{
    public partial class FDTSite : CanvasIFrameMasterPage
    {
        // helper for testing
        public readonly bool TestMode;

        public FDTSite()
        {
            string testModeValue = WebConfigurationManager.AppSettings["TestMode"] ?? true.ToString();
            TestMode = Convert.ToBoolean(testModeValue);

            if (!TestMode)
            {
                RequireLogin = true;
                RequiredPermissions = new List<Enums.ExtendedPermissions> {Enums.ExtendedPermissions.publish_stream};
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += Page_Load;
        }

        private void Page_Load(object sender, EventArgs args)
        {
            adMarkup.Controls.Add(new LiteralControl(Settings.Default.AdMarkup));
        }
    }
}