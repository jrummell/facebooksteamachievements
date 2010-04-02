using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace SteamAchievements
{
    public partial class AdminLogin : System.Web.UI.Page
    {
        protected void login_Authenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = FormsAuthentication.Authenticate(login.UserName, login.Password);
        }
    }
}
