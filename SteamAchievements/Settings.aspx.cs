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
using Microsoft.Practices.Unity;
using SteamAchievements.Services;

namespace SteamAchievements
{
    public partial class Settings : FacebookPage
    {
        private IUserService _userService;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += PageLoad;
            Unload += PageUnload;

            _userService = Container.Resolve<IUserService>();
        }

        private void PageUnload(object sender, EventArgs e)
        {
            _userService.Dispose();
        }

        private void PageLoad(object sender, EventArgs e)
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

            User user = _userService.GetUser(FacebookSettings.FacebookUserId);

            if (user != null)
            {
                autoUpdateCheckBox.Checked = user.AutoUpdate;
                publishDescriptionCheckBox.Checked = user.PublishDescription;
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
                                PublishDescription = publishDescriptionCheckBox.Checked,
                                SteamUserId = steamIdTextBox.Text
                            };

            bool newUser;
            try
            {
                newUser = _userService.GetUser(user.FacebookUserId) == null;
                _userService.UpdateUser(user);
            }
            catch (DuplicateSteamUserException)
            {
                duplicateErrorScript.Visible = true;
                saveSuccessScript.Visible = false;
                return;
            }

            if (newUser)
            {
                using (IAchievementService service = Container.Resolve<IAchievementService>())
                {
                    service.UpdateNewUserAchievements(user);
                }
            }

            saveSuccessScript.Visible = true;
            duplicateErrorScript.Visible = false;
        }
    }
}