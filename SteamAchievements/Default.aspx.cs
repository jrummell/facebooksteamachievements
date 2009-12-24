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

        protected string SteamUserId
        {
            get; private set;
        }
    }
}