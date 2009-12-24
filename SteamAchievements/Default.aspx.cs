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
using SteamAchievements.Data;

namespace SteamAchievements
{
    public partial class Default : Page
    {
        protected string SteamUserId { get; private set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += Page_Load;
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Master.RequireLogin = true;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            SteamUserId = "NullReference";

            return;

            try
            {
                AchievementManager service = new AchievementManager();
                SteamUserId = service.GetSteamUserId(Master.Api.Session.UserId);
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}