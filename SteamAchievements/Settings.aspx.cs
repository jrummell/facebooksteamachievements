﻿#region License

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
using SteamAchievements.Services;

namespace SteamAchievements
{
    public partial class Settings : FacebookPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += Page_Load;
        }

        private void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                return;
            }

            if (FacebookSettings == null)
            {
                return;
            }

            steamIdTextBox.Text = FacebookSettings.SteamUserId;

            using (IUserService service = new UserService())
            {
                User user = service.GetUser(FacebookSettings.FacebookUserId);

                if (user != null)
                {
                    autoUpdateCheckBox.Checked = user.AutoUpdate;
                }
            }
        }

        protected void SaveSettingsButtonClick(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }

            if (FacebookSettings == null)
            {
                return;
            }

            User user = new User
                            {
                                FacebookUserId = FacebookSettings.FacebookUserId,
                                AccessToken = FacebookSettings.AccessToken,
                                AutoUpdate = autoUpdateCheckBox.Checked,
                                SteamUserId = steamIdTextBox.Text
                            };

            bool newUser;
            try
            {
                using (IUserService service = new UserService())
                {
                    newUser = service.GetUser(user.FacebookUserId) == null;
                    service.UpdateUser(user);
                }
            }
            catch (DuplicateSteamUserException)
            {
                duplicateErrorScript.Visible = true;
                saveSuccessScript.Visible = false;
                return;
            }

            if (newUser)
            {
                using (IAchievementService service = new AchievementService())
                {
                    service.UpdateNewUserAchievements(user);
                }
            }

            saveSuccessScript.Visible = true;
            duplicateErrorScript.Visible = false;
        }
    }
}