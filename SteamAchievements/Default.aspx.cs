using System;
using System.Web.UI;
using SteamAchievements.Data;

namespace SteamAchievements
{
    public partial class Default : Page
    {
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
            try
            {
                AchievementService service = new AchievementService();
                string steamUserId = service.GetSteamUserId(Master.Api.Session.UserId);

                if (!String.IsNullOrEmpty(steamUserId))
                {
                    achievementsRepeater.DataSource = service.GetAchievements(steamUserId, 1);
                    achievementsRepeater.DataBind();
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
    }
}