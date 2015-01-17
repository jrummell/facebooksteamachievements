using System.IO;
using System.Web.Mvc;
using MarkdownSharp;
using SteamAchievements.Web.Models;

namespace SteamAchievements.Web.Controllers
{
    public class HelpController : Controller
    {
        public ActionResult Index()
        {
            return View("Topic", GetModel("Help"));
        }

        public ActionResult Topic(string id)
        {
            var model = GetModel(id);
            if (model == null)
            {
                return HttpNotFound();
            }

            return View(model);
        }

        private MarkdownModel GetModel(string id)
        {
            var fileName = Path.Combine(Server.MapPath("~/App_Data/Documents"), id + ".md");
            if (!System.IO.File.Exists(fileName))
            {
                return null;
            }

            var content = System.IO.File.ReadAllText(fileName);

            Markdown markdown = new Markdown();
            var transformedContent = markdown.Transform(content);

            var model = new MarkdownModel {Content = transformedContent};
            return model;
        }
    }
}