using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace SteamAchievements.Web.Models
{
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string userName)
        {
            IIdentity identity = HttpContext.Current.User.Identity;
            if (!identity.IsAuthenticated)
            {
                FormsAuthentication.SetAuthCookie(userName, true);
            }
            else if (identity.Name != userName)
            {
                FormsAuthentication.SignOut();
                FormsAuthentication.SetAuthCookie(userName, true);
            }
        }
    }
}