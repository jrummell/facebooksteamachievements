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
using SteamAchievements.Services;
using System.Collections.Generic;

namespace SteamAchievements.Admin
{
    public partial class AddGame : Page
    {
        protected void addButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }

            Game game = new Game
                            {
                                Abbreviation = abbreviationTextBox.Text,
                                Name = nameTextBox.Text
                            };

            using (SteamCommunityManager community = new SteamCommunityManager())
            using (AchievementManager manager = new AchievementManager())
            { 
                manager.AddGame(game);

                IEnumerable<Achievement> achievements = 
                    community.GetAchievements(steamUserIdTextBox.Text, game);

                manager.AddAchievements(game.Id, achievements);
            }

            Response.Redirect("~/Admin");
        }
    }
}