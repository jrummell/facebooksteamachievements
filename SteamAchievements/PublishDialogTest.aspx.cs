using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
                + FacebookClientId + "&redirect_uri=" + FacebookCallbackUrl + "&type=user_agent&display=popup";

                Response.Redirect(redirectUrl);
            }
        }
    }
}