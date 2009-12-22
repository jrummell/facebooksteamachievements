using System;
using System.ServiceModel.Syndication;
using System.Web;
using System.Xml;
using SteamAchievements.Data;

namespace SteamAchievements
{
    public class AchievementsFeed : IHttpHandler
    {
        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            string steamId = context.Request.QueryString["user"];
            string gameIdString = context.Request.QueryString["game"];

            if (String.IsNullOrEmpty(steamId) || String.IsNullOrEmpty(gameIdString))
            {
                context.Response.Write("missing user or game");
            }

            int gameId;
            try
            {
                gameId = Convert.ToInt32(gameIdString);
            }
            catch (FormatException)
            {
                context.Response.Write("invalid game");
                return;
            }

            context.Response.ContentType = "application/rss+xml";

            XmlWriter writer = new XmlTextWriter(context.Response.Output);

            AchievementService service = new AchievementService();
            AchievementCollection achievements = service.GetAchievements(steamId, gameId);
            SyndicationFeed feed = achievements.ToSyndicationFeed();

            feed.SaveAsRss20(writer);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}