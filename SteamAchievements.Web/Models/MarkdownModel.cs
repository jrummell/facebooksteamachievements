using System.ComponentModel.DataAnnotations;

namespace SteamAchievements.Web.Models
{
    public class MarkdownModel
    {
        [DataType(DataType.Html)]
        public string Content { get; set; }
    }
}