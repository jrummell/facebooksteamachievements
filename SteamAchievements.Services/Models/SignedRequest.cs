namespace SteamAchievements.Services.Models
{
    public class SignedRequest
    {
        public long UserId { get; set; }

        public string AccessToken { get; set; }
    }
}