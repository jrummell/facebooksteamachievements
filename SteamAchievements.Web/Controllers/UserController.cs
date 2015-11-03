using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using SteamAchievements.Data;

namespace SteamAchievements.Web.Controllers
{
    public abstract class UserController : Controller
    {
        private SignInManager<steam_User, int> _signInManager;
        private UserManager<steam_User, int> _userManager;
        //TODO: Dependency Injection for ASP.NET Identity

        public SignInManager<steam_User, int> SignInManager
        {
            get { return _signInManager ?? HttpContext.GetOwinContext().Get<SignInManager<steam_User, int>>(); }
            private set { _signInManager = value; }
        }

        public UserManager<steam_User, int> UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<UserManager<steam_User, int>>(); }
            private set { _userManager = value; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}