using System;
using System.Collections.Generic;
using Facebook.Schema;
using Facebook.Web;

namespace SteamAchievements
{
    public partial class Site : CanvasFBMLMasterPage
    {
        public Site()
        {
            RequiredPermissions = new List<Enums.ExtendedPermissions> {Enums.ExtendedPermissions.publish_stream};
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!String.IsNullOrEmpty(Request.Params["fb_sig_in_profile_tab"]))
            //{
            //    header.Visible = false;
            //    css.Text = FBMLControlRenderer.RenderFBML("~/controls/FBMLCSS.ascx");
            //}
            //else
            //{
            //    css.Text = String.Format(
            //        "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}css/page.css?id={1}\" />", callback,
            //        cssVersion);
            //    js.Text = FBMLControlRenderer.RenderFBML("~/controls/FBMLJS.ascx");
            //}
        }
    }
}