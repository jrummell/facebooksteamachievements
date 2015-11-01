using Microsoft.Owin;
using Owin;
using SteamAchievements.Web;

[assembly: OwinStartup(typeof (Startup))]

namespace SteamAchievements.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}