using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using SteamAchievements.Data;

namespace SteamAchievements.Services
{
    [ServiceContract]
    public interface IAchievementService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Get?steamUserId={steamUserId}&gameId={gameId}",
            ResponseFormat = WebMessageFormat.Json)]
        AchievementCollection GetAchievements(string steamUserId, int gameId);

        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        Collection<Game> GetGames(); // returning IEnumerable<GameDTO> causes a serialization exception

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "Get?steamUserId={steamUserId}",
            ResponseFormat = WebMessageFormat.Json)]
        void UpdateAchievements(string steamUserId);
    }
}