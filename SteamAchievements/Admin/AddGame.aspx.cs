using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SteamAchievements.Data;

namespace SteamAchievements.Admin
{
    public partial class AddGame : System.Web.UI.Page
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

            AchievementManager manager = new AchievementManager();
            manager.AddGame(game);

            Response.Redirect("~/Admin");
        }
    }
}
